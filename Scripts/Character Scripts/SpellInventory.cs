using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInventory : MonoBehaviour {

    public List<Projectile> projectileSpells;
    public List<Beam> beamSpells;
    public List<Wall> wallSpells;
    public List<Ring> ringSpells;
    public List<Enhancement> enhancementSpells;
    public List<Utility> utilitySpells;
    public List<Forbidden> forbiddenSpells;

    private List<Spell> allSpells = new List<Spell>();


    void Start() {
        CompileSpells();
    }

    void CompileSpells() {
        allSpells.Clear();
        foreach(Spell spell in projectileSpells) {
            allSpells.Add(spell);
        }
        foreach (Spell spell in beamSpells) {
            allSpells.Add(spell);
        }
        foreach (Spell spell in wallSpells) {
            allSpells.Add(spell);
        }
        foreach (Spell spell in ringSpells) {
            allSpells.Add(spell);
        }
        foreach (Spell spell in enhancementSpells) {
            allSpells.Add(spell);
        }
        foreach (Spell spell in utilitySpells) {
            allSpells.Add(spell);
        }
        foreach (Spell spell in forbiddenSpells) {
            allSpells.Add(spell);
        }
    }

    public List<Spell> Spells() {
        return allSpells;
    }

    public void Add(Spell spell) {
        switch (spell.type) {
            case Spell.Type.Beam:
                Beam b = spell as Beam;
                beamSpells.Add(b);
                break;
            case Spell.Type.Enhancement:
                Enhancement e = spell as Enhancement;
                enhancementSpells.Add(e);
                break;
            case Spell.Type.Forbidden:
                Forbidden f = spell as Forbidden;
                forbiddenSpells.Add(f);
                break;
            case Spell.Type.Projectile:
                Projectile p = spell as Projectile;
                projectileSpells.Add(p);
                break;
            case Spell.Type.Ring:
                Ring r = spell as Ring;
                ringSpells.Add(r);
                break;
            case Spell.Type.Utility:
                Utility u = spell as Utility;
                utilitySpells.Add(u);
                break;
            case Spell.Type.Wall:
                Wall w = spell as Wall;
                wallSpells.Add(w);
                break;
        }
        CompileSpells();
    }
}
