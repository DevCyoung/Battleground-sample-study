using UnityEngine;
using UnityEngine.UI;




/// <summary>
/// 무기를 획득하면 획득한 무기를 UI를 통해 보여주고 
/// 현재 잔탕량과 소지할수있는 총알량을 출력 
/// </summary>
public class WeaponUIManager : MonoBehaviour
{
    public Color bulletColor = Color.white;
    public Color emptyBulletColor = Color.black;
    private Color noBulletColor; // 투명하게 색표시

    [SerializeField]
    private Image weaponHUD;
    [SerializeField]
    private GameObject bulletMag;
    [SerializeField]
    private Text totalBulletsHUD;

    private void Start()
    {
        noBulletColor = new Color(0f, 0f, 0f, 0f);
        if(weaponHUD == null)
        {
            weaponHUD = transform.Find("WeaponHUD/Weapon").GetComponent<Image>();
        }
        if(bulletMag == null)
        {
            bulletMag = transform.Find("WeaponHUD/Data/Mag").gameObject;
        }
        if(totalBulletsHUD == null)
        {
            totalBulletsHUD = transform.Find("WeaponHUD/Data/Label").GetComponent<Text>();
        }

        Toggle(false);
    }


    public void Toggle(bool active)
    {
        weaponHUD.transform.parent.gameObject.SetActive(active);

    }

    public void UpdateWeaponHUD(Sprite weaponSprite , int bulletLeft , int fullMag , int ExtraBullet)
    {
        if(weaponSprite != null && weaponHUD.sprite != weaponSprite)
        {
            weaponHUD.sprite = weaponSprite;
            weaponHUD.type = Image.Type.Filled;
            weaponHUD.fillMethod = Image.FillMethod.Horizontal;
        }

        int bulletCount = 0;
        foreach (Transform bullet in bulletMag.transform)
        {
            //잔탄
            if (bulletCount < bulletLeft)
            {
                bullet.GetComponent<Image>().color = bulletColor;

            }
            else if ( bulletCount >= fullMag)
            {
                bullet.GetComponent<Image>().color = noBulletColor;
            }
            else
            {
                bullet.GetComponent<Image>().color = emptyBulletColor;
            }
            bulletCount++;
        }

        totalBulletsHUD.text = bulletLeft + "/" + ExtraBullet;

    }










}
