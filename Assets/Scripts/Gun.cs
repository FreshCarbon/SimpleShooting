using System.Collections;
using UnityEngine;

// ���� �����Ѵ�
public class Gun : MonoBehaviour
{
    // ���� ���¸� ǥ���ϴµ� ����� Ÿ���� �����Ѵ�
    public enum State
    {
        Ready, // �߻� �غ��
        Empty, // źâ�� ��
        Reloading // ������ ��
    }

    public State state { get; private set; } // ���� ���� ����        

    public Transform fireTransform; // �Ѿ��� �߻�� ��ġ

    public ParticleSystem muzzleFlashEffect; // �ѱ� ȭ�� ȿ��
    //public ParticleSystem shellEjectEffect; // ź�� ���� ȿ��

    private LineRenderer bulletLineRenderer; // �Ѿ� ������ �׸��� ���� ������

    public GunData gunData; //���� ���� ������

    private AudioSource gunAudioPlayer; // �� �Ҹ� �����
    public AudioClip shotClip; // �߻� �Ҹ�
    public AudioClip reloadClip; // ������ �Ҹ�

    private float fireDistance = 50f; // �����Ÿ�

    //public int ammoRemain; // ���� ��ü ź��
    public int magAmmo; // ���� źâ�� �����ִ� ź��

    //public float timeBetFire = 0.12f; // �Ѿ� �߻� ����
    public float reloadTime = 1.8f; // ������ �ҿ� �ð�
    private float lastFireTime; // ���� ���������� �߻��� ����

    private void Awake()
    {
        // ����� ������Ʈ���� ������ ��������
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        //����� ��츦 �� ���� ����, ���� �������� ����� ���� ���� 2�� ����
        bulletLineRenderer.positionCount = 2;
        // ���� �������� ��Ȱ��ȭ �ν����Ϳ��� �ϱ� ������ �ڵ�� Ȯ���ϰ�
        bulletLineRenderer.enabled = false;
    }

    //������Ʈ�� Ȱ��ȭ �� �� ����� ���� Ȱ��ȭ �� �� ���� ���¿� �⺻ ź�� �ʱ�ȭ
    private void OnEnable()
    {
        //��ü ���� ź�� ���� �ʱ�ȭ
        //ammoRemain = gunData.startAmmoRemain;
        //���� źâ�� ���� ä���
        magAmmo = gunData.magCapacity;

        //���� ���� ���¸� ���� �� �غ� �� ���·� ����
        state = State.Ready;
        //���������� ���� �� ���� �ʱ�ȭ
        lastFireTime = 0;

        // ����� Ŭ���� ����� �Ҵ�Ǿ����� Ȯ��
        if (shotClip == null)
        {
            Debug.LogError("ShotClip�� �Ҵ���� �ʾҽ��ϴ�.");
        }

        if (reloadClip == null)
        {
            Debug.LogError("ReloadClip�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    // �߻� �õ�
    public void Fire()
    {
        //���� ���°� �߻� ������ ����, ������ �� �߻� �������� gunData.timeBetFire �̻��� �ð��� ����
        if (state == State.Ready && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            //������ �� �߻� ���� ����
            lastFireTime = Time.time;
            //���� �߻� ó��
            Shot();
        }
    }

    // ���� �߻� ó��
    private void Shot()
    {
        //����ĳ��Ʈ�� ���� �浹 ������ �����ϴ� �����̳�
        RaycastHit hit;
        //ź���� ���� ���� ������ ����
        Vector3 hitPosition = Vector3.zero;

        // ����ĳ��Ʈ(���� ����, ����, �浹���� �����̳�, �����Ÿ�)
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            //���̰� � ��ü�� �浹�� ��� �浹�� �������κ��� IDamageable �������� �õ�
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            //�������κ��� IDamageable ������Ʈ�� �������µ��� �����ߴٸ�
            if (target != null)
            {
                //������ OnDamage �Լ��� ������� ���濡 ����� �ֱ�
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }
            hitPosition = hit.point;
        }
        else
        {
            //���̰� �ٸ� ��ü�� �浹���� �ʾҵ���
            //ź���� �ִ� �����Ÿ����� ���ư��� ���� ��ġ�� �浹 ��ġ�� ���
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        //�߻� ����Ʈ ��� ����
        StartCoroutine(ShotEffect(hitPosition));

        //���� ź�˼� -1
        magAmmo--;
        if (magAmmo <= 0)
        {
            //���� ź���� ���ٸ� ���� ���¸� empty�� ����
            state = State.Empty;
        }
    }

    // �߻� ����Ʈ�� �Ҹ��� ����ϰ� �Ѿ� ������ �׸���
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        //�ѱ� ȭ�� ȿ�� ���
        muzzleFlashEffect.Play();
        //ź�� ���� ȿ�� ���
        //shellEjectEffect.Play();

        //�Ѱ� �Ҹ� ���, playOneShot�� �Ҹ��� �������� �ʰ� ��ø�Ͽ� ����ϰ� ����
        if (shotClip != null && gunAudioPlayer != null)
        {
            gunAudioPlayer.PlayOneShot(shotClip);
        }
        else
        {
            Debug.LogError("ShotClip or AudioSource null");
        }

        //���� �������� �ѱ��� ��ġ, transform�� ���� �ѱ��� ��ġ �޾ƿ�
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        //���� �� ���� �Է����� ���� �浹 ��ġ , 0���� 1 ������ ���� �׸�
        bulletLineRenderer.SetPosition(1, hitPosition);
        //���� �������� Ȱ��ȭ�Ͽ� ź�� ������ �׸�
        bulletLineRenderer.enabled = true;

        // 0.03�� ���� ��� ó���� ���
        yield return new WaitForSeconds(0.03f);

        // ���� �������� ��Ȱ��ȭ�Ͽ� �Ѿ� ������ �����
        bulletLineRenderer.enabled = false;
    }

    // ������ �õ�
    public bool Reload()
    {
        if (state == State.Reloading || /*ammoRemain <= 0 ||*/ magAmmo >= gunData.magCapacity)
        {
            //������ ���̰ų� �Ѿ��� ������ ��쿡��
            return false;
        }
        StartCoroutine(ReloadRoutine());
        return true;
    }

    // ���� ������ ó���� ����
    private IEnumerator ReloadRoutine()
    {
        // ���� ���¸� ������ �� ���·� ��ȯ
        state = State.Reloading;

        if (reloadClip != null && gunAudioPlayer != null)
        {
            gunAudioPlayer.PlayOneShot(reloadClip);
        }
        else
        {
            Debug.LogError("ReloadClip �Ǵ� AudioSource�� null�Դϴ�.");
        }

        // ������ �ҿ� �ð� ��ŭ ó���� ����
        yield return new WaitForSeconds(gunData.reloadTime);

        magAmmo = gunData.magCapacity;
        state = State.Ready;
        
        //źâ�� ä�� ź�� ���
        /*int ammoToFill = gunData.magCapacity - magAmmo;

        if (ammoRemain > 0)
        {
            if (ammoToFill <= ammoRemain)
            {
                // ������ �Ѿ��� ���� �ִ� ���
                magAmmo += ammoToFill;
                ammoRemain -= ammoToFill;
                state = State.Ready;
            }
            else
            {
                // ���� �Ѿ��� źâ�� ä���� ���ϴ� ���
                magAmmo += ammoRemain;
                ammoRemain = 0;
                state = State.Ready;
            }
        }*/
    }
}
