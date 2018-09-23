using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFireballScript : MonoBehaviour {

    private bool isActive;
    private Vector3 startLocation;
    private GameObject caster;
    public SpriteRenderer sr;
    private Projectile spell;
    public bool IsActive { get; set; }
    public Vector3 dir;


    public void SetSpell(Spell spell, Vector3 location, Vector3 direction, GameObject caster) {
        this.spell = spell as Projectile;
        sr.sprite = spell.sprite;
        this.caster = caster;
        gameObject.transform.position = location;
        startLocation = location;
        dir = direction.normalized;
        isActive = true;
    }


    void Update() {
        if (!isActive) {
            return;
        }

        transform.position += (dir * Time.deltaTime * spell.speed);
        if(Vector3.Distance(startLocation, gameObject.transform.position) >= spell.distance) {
            Destroy(gameObject);
        }

    }



   void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject == caster) { //dont mess with caster, might need to change
            return;
        }
        if(collision.gameObject.layer == 8 || collision.gameObject.layer == 9) { //if the collision is with a player or npc
            //print("Hit");
            collision.gameObject.GetComponent<CharacterInfo>().LoseHealth(spell.damage);
            collision.gameObject.GetComponent<CharacterInfo>().TryInsta(spell.killChance);
            Destroy(gameObject);


        }
    }


}
