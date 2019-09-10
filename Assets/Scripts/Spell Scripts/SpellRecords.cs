using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellRecords : MonoBehaviour{

    public Vector3 dir;
    public GameObject caster;
    public bool isActive;
    protected Vector3 startLocation;
    public Vector3 StartLocation { get; }
    public SpriteRenderer sr;
    public Spell spell;

    public abstract void SetSpell(Spell spell, Vector3 location, Vector3 direction, GameObject caster);
    public abstract void SetSpell(Spell spell, Vector3 location, Vector3 currentLocation, Vector3 direction, GameObject caster);

}
