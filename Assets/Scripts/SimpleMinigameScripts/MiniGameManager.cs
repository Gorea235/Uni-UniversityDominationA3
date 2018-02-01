using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class MiniGameManager : MonoBehaviour
    {

        #region Unity bindings

        public GameObject player;

        #endregion

        #region Private fields

        float timer = 60.0f;

        #endregion

        #region Properties

        public int BonusKnowledge { get; set; }
        public int BonusBeer { get; set; }
        public MiniGamePlayer MiniPlayer { get { return player.GetComponent<MiniGamePlayer>(); } }

        #endregion


        #region MonoBehaviour


        void Start()
        {
          //do nuffin
        }

        void Update()
        {

            //decrement timer;
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                //handle game exit
            }


        }

        #endregion
    }