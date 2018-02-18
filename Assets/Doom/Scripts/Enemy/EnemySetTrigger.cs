using System;
using UnityEngine;

/// <summary>
/// Allows enemies to be grouped together and have states applied and fetched all at once, and
/// have events trigger on states of the entire set.
/// </summary>
public class EnemySetTrigger : MonoBehaviour
{
    #region Unity Bindings

    public EnemySet[] m_enemySets;
    public int m_currentSet = 0;
    public TriggerAction m_actionOnTrigger;
    public TiggerRequirement m_requirementForTrigger;
    public bool m_loopTrigger;

    #endregion

    #region Data Structures

    /// <summary>
    /// The action that should be applied to the next set upon an trigger.
    /// </summary>
    public enum TriggerAction
    {
        Activate,
        Deactive,
        EnableChasing,
        DisableChasing
    }

    /// <summary>
    /// The requirement for the trigger action to be fired.
    /// </summary>
    public enum TiggerRequirement
    {
        None,
        AllAlive,
        AllDead
    }

    /// <summary>
    /// A set of enemies.
    /// </summary>
    [Serializable]
    public class EnemySet
    {
        #region Unity Bindings

        public EnemyController[] m_enemies;
        public bool m_defaultActive;
        public bool m_defaultChasing;
        public UnityEngine.Object m_triggerOnAllDead;
        public Action m_triggerAction;

        #endregion

        #region Data Structures

        /// <summary>
        /// The action to apply to <see cref="m_triggerOnAllDead"/> once all the enemies in the set
        /// have died.
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Fire the DoActivateTrigger function of the <see cref="UnityEngine.Object"/>.
            /// </summary>
            Trigger,
            /// <summary>
            /// Active the <see cref="UnityEngine.Object"/>.
            /// </summary>
            Activate,
            /// <summary>
            /// Deactivate the <see cref="UnityEngine.Object"/>.
            /// </summary>
            Deactivate,
            /// <summary>
            /// Enable the <see cref="Behaviour"/>.
            /// </summary>
            Enable,
            /// <summary>
            /// Disable the <see cref="Behaviour"/>.
            /// </summary>
            Disable
        }

        #endregion

        #region Private Fields

        const string _notEnemiesException = "No enemies added to set";
        int _amountAlive;

        #endregion

        #region Handlers

        /// <summary>
        /// Handles the event on enemy death.
        /// </summary>
        void Enemy_OnDeath(object sender, EventArgs e)
        {
            _amountAlive--;
            // if no more of the enemies are alive, then trigger the action
            if (_amountAlive == 0 && m_triggerOnAllDead != null)
            {
                Behaviour objBehaviour = m_triggerOnAllDead as Behaviour;
                GameObject objGameObject = m_triggerOnAllDead as GameObject;

                switch (m_triggerAction)
                {
                    case Action.Trigger:
                        objGameObject?.BroadcastMessage("DoActivateTrigger");
                        break;
                    case Action.Activate:
                        objGameObject?.SetActive(true);
                        break;
                    case Action.Deactivate:
                        objGameObject?.SetActive(false);
                        break;
                    case Action.Enable:
                        if (objBehaviour != null)
                            objBehaviour.enabled = true;
                        break;
                    case Action.Disable:
                        if (objBehaviour != null)
                            objBehaviour.enabled = false;
                        break;
                }
            }
        }

        #endregion

        #region Helpers
        internal void Awake()
        {
            _amountAlive = m_enemies.Length;
            // apply default states to set
            foreach (EnemyController enemy in m_enemies)
            {
                enemy.IsActive = m_defaultActive;
                enemy.IsChasing = m_defaultChasing;
                Debug.Log(string.Format("Set {0}", enemy.name));
            }
        }
        internal void OnEnable()
        {
            foreach (EnemyController enemy in m_enemies)
                enemy.OnDeath += Enemy_OnDeath;
        }

        internal void OnDisable()
        {
            foreach (EnemyController enemy in m_enemies)
                enemy.OnDeath -= Enemy_OnDeath;
        }

        /// <summary>
        /// Get or set the active state of all the enemies in the set.
        /// </summary>
        public bool Active
        {
            get
            {
                if (m_enemies.Length == 0)
                    throw new IndexOutOfRangeException(_notEnemiesException);
                return m_enemies[0].IsActive;
            }
            set
            {
                foreach (EnemyController enemy in m_enemies)
                    enemy.IsActive = value;
            }
        }

        /// <summary>
        /// Get or set the chasing state of all the enemies in the set.
        /// </summary>
        /// <returns></returns>
        public bool Chasing
        {
            get
            {
                if (m_enemies.Length == 0)
                    throw new IndexOutOfRangeException(_notEnemiesException);
                return m_enemies[0].IsChasing;
            }
            set
            {
                foreach (EnemyController enemy in m_enemies)
                    enemy.IsChasing = value;
            }
        }

        /// <summary>
        /// Checks whether all of the enemies in the set are alive.
        /// </summary>
        public bool AllAlive
        {
            get
            {
                foreach (EnemyController enemy in m_enemies)
                    if (enemy.IsDead)
                        return false;
                return true;
            }
        }

        /// <summary>
        /// Checks whether all of the enemies in the set are dead.
        /// </summary>
        public bool AllDead
        {
            get
            {
                foreach (EnemyController enemy in m_enemies)
                    if (!enemy.IsDead)
                        return false;
                return true;
            }
        }

        #endregion
    }

    #endregion

    #region Private Fields


    #endregion

    #region MonoBevaviour

    void Awake()
    {
        foreach (EnemySet eset in m_enemySets)
            eset.Awake();
    }

    void OnEnable()
    {
        foreach (EnemySet eset in m_enemySets)
            eset.OnEnable();
    }

    void OnDisable()
    {
        foreach (EnemySet eset in m_enemySets)
            eset.OnDisable();
    }

    void DoActivateTrigger()
    {
        // double check current set against available indexes
        if (m_currentSet >= m_enemySets.Length)
            return;

        // enforce requirement
        switch (m_requirementForTrigger)
        {
            case TiggerRequirement.AllAlive:
                if (!m_enemySets[m_currentSet].AllAlive)
                    return;
                break;
            case TiggerRequirement.AllDead:
                if (!m_enemySets[m_currentSet].AllDead)
                    return;
                break;
        }

        // apply action
        switch (m_actionOnTrigger)
        {
            case TriggerAction.Activate:
                m_enemySets[m_currentSet].Active = true;
                break;
            case TriggerAction.Deactive:
                m_enemySets[m_currentSet].Active = false;
                break;
            case TriggerAction.EnableChasing:
                m_enemySets[m_currentSet].Chasing = true;
                break;
            case TriggerAction.DisableChasing:
                m_enemySets[m_currentSet].Chasing = false;
                break;
        }

        // increament current set
        m_currentSet++;
        // apply looping
        if (m_loopTrigger)
            m_currentSet = m_currentSet % m_enemySets.Length;
    }

    #endregion
}
