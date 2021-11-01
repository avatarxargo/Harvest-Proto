using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bilboard : MonoBehaviour
{
    public Vector3 anchor;
    public HexEntity owner;
    public HexDirection ownerDir;
    public TriDirection ownerTri;
    public HexFlagType ownerType;
    public Camera cam;
    public RectTransform rect;
    public Image image;

    public Text textt;

    public bool expires = true;
    public bool isSnapped = false;

    public float life = 0;
    public float lifespan = 3;
    void Start()
    {
        init();
    }

    public void init() {
        rect = this.GetComponent<RectTransform>();
        image = this.GetComponent<Image>();
        life = 0;
        Vector3 screenPos = cam.WorldToScreenPoint(anchor + new Vector3(0,1+life/lifespan,0));
        rect.transform.position = screenPos;
        rect.sizeDelta = new Vector2 (20, 20);
    }
    void Update()
    {
        if(image) {
            if(isSnapped) {
                Vector2 spot = HexCoordinate.getTriSectionCenter(HexDirectionUtil.rotate(owner.coord,(int)ownerDir),ownerTri);
                anchor = new Vector3(spot.x, 0.65f, spot.y);
            }
            if(expires) {
                life += Time.deltaTime;
                //Vector3 v = target.transform.position - transform.position;
                /*transform.rotation = Quaternion.LookRotation(v);
                transform.position = anchor.transform.position + new Vector3(0,1+life/lifespan,0);
                transform.localScale = new Vector3(0.3f*(1.5f-life/lifespan),0.3f*(1.5f-life/lifespan),0.3f*(1.5f-life/lifespan));
                */
                float opacity = 1;
                if(life/lifespan > 0.5f) {
                    opacity = (lifespan-life)/(lifespan/2.0f);
                }
                image.color = new Color(1,1,1,opacity);

                Vector3 screenPos = cam.WorldToScreenPoint(anchor + new Vector3(0,1+life/lifespan,0));
                rect.transform.position = screenPos;
                if(life > lifespan)
                    Object.Destroy(this.gameObject);
            } else {
                Vector3 screenPos = cam.WorldToScreenPoint(anchor);
                rect.transform.position = screenPos;
            }
        } else {

        }
    }

    public void setText(string arg) {
        if(image != null) {
            Destroy(image);
        }
        if(textt == null) {
            this.gameObject.AddComponent<Text>();
            textt = this.gameObject.GetComponent<Text>();
        }
        textt.text = "hello";
    }
    public void setImage(Sprite sprite) {
        image.sprite = sprite;
    }
    public void setFixedAnchor(HexEntity anch, HexDirection sec, TriDirection dir, bool canExpire = false) {
        owner = anch;
        ownerDir = sec;
        ownerTri = dir;
        expires = canExpire;
        isSnapped = true;
        rect = this.GetComponent<RectTransform>();
    }
}
