using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFireballScript : SpellRecords {

    public Projectile projSpell;

    public override void SetSpell(Spell spell, Vector3 location, Vector3 direction, GameObject caster) {
        this.projSpell = (Projectile)spell;

        this.projSpell.dir = direction.normalized;
        dir = direction.normalized;

        sr.sprite = spell.sprite;
        this.caster = caster;
        gameObject.transform.position = location;
        startLocation = location;
        isActive = true;

        dir = direction.normalized;
        this.caster = caster;
    }

    public override void SetSpell(Spell spell, Vector3 location, Vector3 currentLocation, Vector3 direction, GameObject caster) {
        this.projSpell = (Projectile)spell;

        this.projSpell.dir = direction.normalized;
        dir = direction.normalized;

        sr.sprite = spell.sprite;
        this.caster = caster;
        gameObject.transform.position = currentLocation;
        startLocation = location;
        isActive = true;

        dir = direction.normalized;
        this.caster = caster;
    }

    void Update() {
        if (!isActive) {
            return;
        }

        transform.position += (dir * Time.deltaTime * projSpell.speed);
        if(Vector3.Distance(startLocation, gameObject.transform.position) >= projSpell.distance) {
            DestroySpell();
        }

    }



   void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject == caster) { //dont mess with caster, might need to change
            return;
        }
        if(collision.gameObject.layer == 8 || collision.gameObject.layer == 9) { //if the collision is with a player or npc
            //print("Hit");
            collision.gameObject.GetComponent<CharacterInfo>().LoseHealth(projSpell.damage);
            collision.gameObject.GetComponent<CharacterInfo>().TryInsta(projSpell.killChance);
            DestroySpell();


        }
    }


    public void DestroySpell() {
        SpellManagerScript.ins.RemoveActiveSpell(this.gameObject);
        Destroy(gameObject);
    }

}
