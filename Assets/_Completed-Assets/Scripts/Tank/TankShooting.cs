﻿using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify the different players.
        public Rigidbody m_Shell;                   // Prefab of the shell.
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public Transform turret;                     // 砲塔のTransform
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

        private string m_FireButton;                // The input axis that is used for launching shells.
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

        private void Awake()
        {
            // 子オブジェクトから砲塔を探してアサイン
            if (turret == null)
            {
                Transform turretTransform = transform.Find("TankRenderers/TankTurret"); // 子オブジェクトのパスを指定
                if (turretTransform != null)
                {
                    turret = turretTransform;
                }
                else
                {
                    Debug.LogError("Turret not found. Please assign the turret in the inspector or check the object name.");
                }
            }
        }

        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }

        private void Start()
        {
            // The fire axis is based on the player number.
            m_FireButton = "Fire" + m_PlayerNumber;

            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }

        private void Update()
        {
           // 矢印が車体の根元から伸びるように、車体の位置を基準にして矢印の位置を調整
            Vector3 aimSliderPosition = turret.position + turret.forward * 6.0f; // 砲塔の前方に少し伸ばす
            m_AimSlider.transform.position = aimSliderPosition + turret.up*0.5f; // 砲塔の高さに合わせる
            // 矢印が砲塔の方向を向くように回転を砲塔に合わせる
            m_AimSlider.transform.rotation = turret.rotation * Quaternion.Euler(90, 0, 0);  // 砲塔の回転に合わせて矢印を表示

            // The slider should have a default value of the minimum launch force.
            m_AimSlider.value = m_MinLaunchForce;

            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire();
            }
        else if (Input.GetButtonDown(m_FireButton))
            {
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }       
            else if (Input.GetButton(m_FireButton) && !m_Fired)
            {
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
            {
                Fire();
            }
        }


public void Fire()
{
    m_Fired = true;

    // Debugging to check for null references
    if (m_Shell == null) Debug.LogError("Shell is null");
    if (m_FireTransform == null) Debug.LogError("FireTransform is null");
    if (turret == null) Debug.LogError("Turret is null");

    // 砲弾をturretの先端からさらに前方に位置調整
     Vector3 firePosition = m_FireTransform.position + new Vector3(0, 0.85f, 0); // y方向に少し上に
    Rigidbody shellInstance = Instantiate(m_Shell, firePosition, turret.rotation) as Rigidbody;

    shellInstance.velocity = m_CurrentLaunchForce * turret.forward;

    m_ShootingAudio.clip = m_FireClip;
    m_ShootingAudio.Play();

    m_CurrentLaunchForce = m_MinLaunchForce;
}
    }
}
