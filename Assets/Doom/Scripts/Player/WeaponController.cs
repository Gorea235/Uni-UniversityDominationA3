using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Rng = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class WeaponController : MonoBehaviour
{
    #region Unity Bindings

    public int m_numberBulletsPerShot;
    public float m_spreadRadius;
    /// <summary>
    /// The number of sectors to attempt to spread the shots
    /// over. This means for a number of bullets > than this, there will be at
    /// least 1 bullet in each sector, making the spread guaranteed to be more
    /// even.
    /// </summary>
    public int m_sectorSplit = 4;
    public float m_damagePerShot;
    public float m_maxRange;
    public float m_recoilTime;
    public AudioClip m_fireSound;
    public GameObject m_bulletTrail;
    public GameObject m_bulletTrailOriginObj;
    public Image m_reticle;

    #endregion

    #region Private Classes

    /// <summary>
    /// Stores the hits and total damage that will be applied to the hit <see cref="GameObject"/>.
    /// </summary>
    class ObjDamageStore
    {
        /// <summary>
        /// The amount of damage that should be applied.
        /// </summary>
        public float Damage { get; set; }
        /// <summary>
        /// The list of points that the bullet hit so we can draw the bullet trails.
        /// </summary>
        public List<Vector3> BulletHits { get; }

        /// <summary>
        /// Constructs a new <see cref="T:ObjDamageStore"/> with the base damager and hit point.
        /// </summary>
        /// <param name="damage">The intial damage to apply.</param>
        /// <param name="hit">The initial hit on the object.</param>
        public ObjDamageStore(float damage, Vector3 hit)
        {
            Damage = 0;
            BulletHits = new List<Vector3>();
            AddHit(damage, hit);
        }

        /// <summary>
        /// Adds a new hit to the storage.
        /// </summary>
        /// <param name="damage">The damage of the hit.</param>
        /// <param name="hit">The point of the hit.</param>
        public void AddHit(float damage, Vector3 hit)
        {
            Damage += damage;
            BulletHits.Add(hit);
        }
    }

    #endregion

    #region Private Fields

    Vector2 _centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);
    float _sectorSize;
    GameObject _player;
    AudioSource _audioSource;
    float _lastShootTime;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _sectorSize = 360f / m_sectorSplit; // init the size of the sector
        _player = GameObject.Find("FPSController");
        _audioSource = gameObject.GetComponent<AudioSource>();
        float d = m_spreadRadius * 2;
        m_reticle.rectTransform.sizeDelta = new Vector2(d, d);
    }

    void Update()
    {
        // only allow shooting if it's been long enough since the last shot
        if ((Time.realtimeSinceStartup - _lastShootTime >= m_recoilTime) && CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            Vector3[] points = GetSpreadPoints(_centerScreen); // grab aim points
            RaycastHit[] hits = GetHitsFromScreenPoints(points); // performs raycasts
            // we store the total damage for each object hit so it's only applied once
            Dictionary<GameObject, ObjDamageStore> damagesToApply = new Dictionary<GameObject, ObjDamageStore>();

            foreach (RaycastHit hit in hits)
            {
                // only allow for hits within range, with 0 or less being no max range
                if (m_maxRange <= 0 || Vector3.Distance(_player.transform.position, hit.point) <= m_maxRange)
                {
                    if (damagesToApply.ContainsKey(hit.collider.gameObject))
                        damagesToApply[hit.collider.gameObject].AddHit(m_damagePerShot, hit.point);
                    else
                        damagesToApply.Add(hit.collider.gameObject,
                                           new ObjDamageStore(m_damagePerShot, hit.point));
                }
            }

            // loop through hits to apply damage
            foreach (KeyValuePair<GameObject, ObjDamageStore> kv in damagesToApply)
            {
                EnemyController enemy = kv.Key.GetComponent<EnemyController>();
                if (enemy != null) // only apply damage to enemies
                    enemy.Damage(kv.Value.Damage);
                CreateBulletTrails(kv.Value.BulletHits.ToArray()); // create bullet trails regardless since we shot
            }

            // play firing sound
            _audioSource.clip = m_fireSound;
            _audioSource.Play();

            // we just shot, so reset timer
            _lastShootTime = Time.realtimeSinceStartup;
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets all the raycast hits from the given points on the screen.
    /// </summary>
    /// <param name="screenPoints">The screen points to start from.</param>
    /// <returns>The list of hits that were found.</returns>
    RaycastHit[] GetHitsFromScreenPoints(Vector3[] screenPoints)
    {
        RaycastHit hit;
        List<RaycastHit> hits = new List<RaycastHit>();
        foreach (Vector3 point in screenPoints)
            if (GetHitFromScreenPoint(point, out hit))
                hits.Add(hit);
        return hits.ToArray();
    }

    /// <summary>
    /// Performs a raycast from the given screen point.
    /// </summary>
    /// <param name="screenPoint">The screen point to start at.</param>
    /// <param name="hit">The hit that was found.</param>
    /// <returns>Whether the raycast hit something.</returns>
    bool GetHitFromScreenPoint(Vector3 screenPoint, out RaycastHit hit)
    {
        Ray hitScanRay = Camera.main.ScreenPointToRay(screenPoint);
        return Physics.Raycast(hitScanRay, out hit);
    }

    /// <summary>
    /// Attempts to spread shots evenly over spread area.
    /// To do this, we split the area into <see cref="m_sectorSplit"/> sectors,
    /// and randomly pick a position in each for each shot.
    /// 
    /// If the bullet count isn't a multiple of <see cref="m_sectorSplit"/> exactly,
    /// then the remainder is split evenly over the area (i.e. split into 3rds
    /// for a remainder of 3, or over the whole area for a remainder of 1).
    /// 
    /// The positions are chosen by randomly generating the rotation around the sector,
    /// and then randomly generating the distance from the centre. By doing this, we avoid
    /// the issue of having to contrain the X and Y values according to each other, since
    /// rotation and distance are not reliant on each other.
    /// </summary>
    /// <returns>The spread points.</returns>
    internal Vector3[] GetSpreadPoints(Vector3? offset = null, float z = 0)
    {
        Vector3 offsetVector = offset ?? new Vector3();

        // the rotation and distance from centre for each shot
        // Item1 - rotaion, Item2 - distance
        Tuple<float, float>[] points = new Tuple<float, float>[m_numberBulletsPerShot];
        int nextIndex = 0;

        // insert standard sector shots
        int shotsPerQuarter = (int)Mathf.Floor(m_numberBulletsPerShot / (float)m_sectorSplit);
        if (shotsPerQuarter > 0)
            for (int r = 0; r < m_sectorSplit; r++)
                for (int i = 0; i < shotsPerQuarter; i++)
                    InsertSpreadPoint(_sectorSize, r, ref nextIndex, ref points);

        // insert remaining shots
        int remaining = m_numberBulletsPerShot % m_sectorSplit;
        float remainingSectorSize = 360f / remaining;
        for (int r = 0; r < remaining; r++)
            InsertSpreadPoint(remainingSectorSize, r, ref nextIndex, ref points);

        // calculate final shot vectors on screen
        Vector3[] spreadPoints = new Vector3[m_numberBulletsPerShot];
        for (int i = 0; i < m_numberBulletsPerShot; i++)
        {
            // given the rotation and the distance from centre, we calculate the
            // vector that it represents, and then offset it by the given amount
            // to allow translation to the centre of the screen
            spreadPoints[i] = new Vector3(points[i].Item2 * Mathf.Cos(points[i].Item1 * Mathf.Deg2Rad),
                                          points[i].Item2 * Mathf.Sin(points[i].Item1 * Mathf.Deg2Rad),
                                          z) + offsetVector;
        }
        return spreadPoints;
    }

    /// <summary>
    /// Inserts a spread point into the given list.
    /// The rotation and distance are randonly generated in the specified ranges.
    /// </summary>
    /// <param name="sectorSize">The size in degrees of the current sector.</param>
    /// <param name="rotation">
    /// The rotation around the spread circle to start at. A rotation of 0 would be the first sector,
    /// a rotation of 2 would be the sector starting at 180° (if sector split is at 4, making each
    /// sector 90°).
    /// </param>
    /// <param name="nextIndex">The next insertion index (as the list size is pre-calculated.</param>
    /// <param name="points">The current list of points to add to.</param>
    void InsertSpreadPoint(float sectorSize, int rotation, ref int nextIndex, ref Tuple<float, float>[] points)
    {
        points[nextIndex] = Tuple.Create(
            Mathf.LerpAngle(sectorSize * rotation, sectorSize * (rotation + 1), Rng.value), // rotation
            m_spreadRadius * Rng.value); // distance from centre
        nextIndex++;
    }

    /// <summary>
    /// Instantiates a new bullet trail going to each specified point.
    /// Start point is given by <see cref="m_bulletTrailOriginObj"/>
    /// </summary>
    /// <param name="endPoints">The list of end points to make the tails go to.</param>
    void CreateBulletTrails(params Vector3[] endPoints)
    {
        foreach (Vector3 point in endPoints)
            Instantiate(m_bulletTrail).GetComponent<BulletTrailController>().Init(m_bulletTrailOriginObj.transform.position, point);
    }

    #endregion
}
