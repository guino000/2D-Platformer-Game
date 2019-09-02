using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float fireRate = 0f;
    public float Damage = 10;
    public LayerMask whatToHit;

    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitPrefab;

    public float effectSpawnRate = 10;
    private float timeToSpawnEffect = 0;
    private float timeToFire = 0;
    Transform firePoint;

    //Handle camera shaking
    public float camShakeAmt = 0.1f;
    public float camShakeLenght = 0.1f;
    CameraShake cameraShake;

    private void Awake()
    {
        firePoint = transform.Find("firePoint");
        if(firePoint == null)
        {
            Debug.LogError("There is no firePoint gameobject assigned to weapon!");
        }
    }

    private void Start()
    {
        cameraShake = GameMaster.gm.GetComponent<CameraShake>();
        if(cameraShake == null)
        {
            Debug.LogError("No CameraShake script found on GM object.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(fireRate == 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, whatToHit);

        Debug.DrawLine(firePointPosition, (mousePosition - firePointPosition) * 100, Color.cyan);
        if(hit.collider != null)
        {
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Debug.Log("We hit " + hit.collider.name + " and did " + Damage + " damage!.");
        }

        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPosition;
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPosition = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }
            else
            {
                hitPosition = hit.point;
                hitNormal = hit.normal;
            }

            Effect(hitPosition, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect(Vector3 hitPosition, Vector3 hitNormal)
    {
        Transform trail = (Transform) Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation);
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if (lr != null)
        {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPosition);
        }

        Destroy(trail.gameObject, 0.04f);

        if(hitNormal != new Vector3(9999, 9999, 9999))
        {
            Transform hitParticle = (Transform) Instantiate(HitPrefab, hitPosition, Quaternion.FromToRotation(Vector3.right, hitNormal));
            Destroy(hitParticle.gameObject, 1f);
        }

        Transform clone = (Transform) Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation);
        clone.parent = firePoint;
        float size = Random.Range(0.6f, 0.9f);
        clone.localScale = new Vector3(size, size, size);
        Destroy(clone.gameObject, 0.02f);

        //Shake the camera
        cameraShake.Shake(camShakeAmt, camShakeLenght);
    }
}
