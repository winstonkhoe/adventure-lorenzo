using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGun : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }

    public int damage = 15;
    public float range = 100f;
    public float shootInterval = 0.425f;
    public float bulletSpeed = 500f;
    public float bulletDrop = 10f;

    public int clipSize = 15;
    public int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;

    public bool shootToPlayer;
    public EnemyBullet peluru;
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;
    public Transform[] raycastOrigin;

    Ray ray;
    RaycastHit hitInfo;

    List<Bullet> bullets = new List<Bullet>();
    float maxLifeTime = 3f;

    void Start()
    {
        currentAmmo = clipSize;
    }

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;
        return bullet.initialPosition + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;
    }

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        UpdateBullets(Time.deltaTime);
    }

    public void ShootPlayer(Transform destination)
    {
        foreach (Transform rO in raycastOrigin)
        {
            ////o.GetComponent<EnemyBullet>().Setup(transform.forward);
            Vector3 newPosition;
            Bullet bullet;
            if (!shootToPlayer)
            {
                GameObject o = Instantiate(peluru.gameObject, rO.position, Quaternion.identity);
                o.GetComponent<EnemyBullet>().Setup(rO.forward, bulletSpeed);
                bullet = CreateBullet(o.transform.position, o.GetComponent<Rigidbody>().velocity);
                o.gameObject.SetActive(false);
            }
            else
            {
                newPosition = new Vector3(destination.position.x, rO.position.y, destination.position.z);
                Vector3 velocity = (newPosition - rO.position).normalized * bulletSpeed;
                bullet = CreateBullet(rO.position, velocity);
            }
            currentAmmo--;
            foreach (var particle in muzzleFlash)
            {
                particle.Emit(1);
            }
            bullets.Add(bullet);
        }
        FindObjectOfType<AudioManager>().SFXPlay("KyleGunSound");
        
    }

    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }
    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = end - start;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            Player p = hitInfo.transform.GetComponent<Player>();
            if(p != null)
            {
                p.gotHit(damage);
            }
            
            if (bullet.tracer != null)
            {
                bullet.tracer.transform.position = hitInfo.point;
            }
            bullet.time = maxLifeTime;
        }
        else
        {
            if (bullet.tracer != null)
            {
                bullet.tracer.transform.position = end;
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = clipSize;

        isReloading = false;
    }
}
