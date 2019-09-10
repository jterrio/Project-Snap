using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManagerScript : MonoBehaviour {

    public static SpellManagerScript ins;
    public List<Spell> allSpells;
    public List<GameObject> allSpellPrefabs;
    private List<GameObject> allActiveSpells;

	// Use this for initialization
	void Start () {

        //singleton
		if(ins == null) {
            ins = this;
        } else {
            if(ins != this) {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
        allSpells = new List<Spell>(Resources.LoadAll<Spell>("Spells"));
        allSpellPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("SpellPrefabs"));
        allActiveSpells = new List<GameObject>();
    }

    /// <summary>
    /// Cast a spell via an Attack
    /// </summary>
    /// <param name="a"></param>
    /// <param name="caster"></param>
    public void CastSpell(CombatHUDAttack.Attack a, GameObject caster, float angle) {
        foreach (GameObject c in allSpellPrefabs) {
            if (c.GetComponent<SpellRecords>().spell == a.selectedSpell) {
                GameObject s = Instantiate(c);
                switch (a.fireMode) {
                    case CombatHUDAttack.FireMode.FREE:
                        s.GetComponent<SpellRecords>().SetSpell(a.selectedSpell, caster.transform.position, a.attackDirection, caster);
                        break;
                    case CombatHUDAttack.FireMode.POINT:
                        s.GetComponent<SpellRecords>().SetSpell(a.selectedSpell, caster.transform.position, (a.attackPointModePoint - caster.transform.position), caster);
                        break;
                    case CombatHUDAttack.FireMode.TARGET:
                        float currentAngle = Vector3.Angle(Vector2.right, CombatManager.ins.combatHUDAttack.memory[a.attackTarget] - caster.transform.position);
                        if (caster.transform.position.y > CombatManager.ins.combatHUDAttack.memory[a.attackTarget].y) {
                            currentAngle = 360 - currentAngle;
                        }
                        currentAngle += angle;
                        if (currentAngle > 360) {
                            currentAngle -= 360;
                        } else if (currentAngle < 0) {
                            currentAngle += 360;
                        }
                        float distance = Vector3.Distance(caster.transform.position, CombatManager.ins.combatHUDAttack.memory[a.attackTarget]);
                        Vector3 newVec = new Vector3(caster.transform.position.x + distance * Mathf.Cos(currentAngle * Mathf.Deg2Rad), caster.transform.position.y + distance * Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                        s.GetComponent<SpellRecords>().SetSpell(a.selectedSpell, caster.transform.position, newVec - caster.transform.position, caster);
                        break;
                    default:
                        s.GetComponent<SpellRecords>().SetSpell(a.selectedSpell, caster.transform.position, a.attackDirection, caster);
                        break;
                }
                AddActiveSpell(s);
                return;
            }
        }


        
        
    }

    /// <summary>
    /// Cast a spell via the Spell
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="location"></param>
    /// <param name="dir"></param>
    /// <param name="caster"></param>
    public void CastSpell(Spell spell, Vector3 location, Vector3 dir, GameObject caster) {
        foreach(GameObject c in allSpellPrefabs) {
            if(c.GetComponent<SpellRecords>().spell.ID == spell.ID) {
                GameObject s = Instantiate(c);
                s.GetComponent<SpellRecords>().SetSpell(spell, location, dir, caster);
                AddActiveSpell(s);
                return;
            }
        }
    }

    /// <summary>
    /// Cast a spell via the Spell that was active before
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="location"></param>
    /// <param name="dir"></param>
    /// <param name="caster"></param>
    public void CastSpell(Spell spell, Vector3 location, Vector3 currentLocation, Vector3 dir, GameObject caster) {
        foreach (GameObject c in allSpellPrefabs) {
            if (c.GetComponent<SpellRecords>().spell.ID == spell.ID) {
                GameObject s = Instantiate(c);
                s.GetComponent<SpellRecords>().SetSpell(spell, location, currentLocation, dir, caster);
                AddActiveSpell(s);
                return;
            }
        }
    }



    public Spell GetSpellFromID(int i) {
        foreach(Spell s in allSpells) {
            if(s.ID == i) {
                return Instantiate(s);
            }
        }
        return Instantiate(allSpells[0]);
    }

    public void AddActiveSpell(GameObject c) {
        allActiveSpells.Add(c);
    }

    public void RemoveActiveSpell(GameObject c) {
        allActiveSpells.Remove(c);
    }



    public class ActiveSpellData {
        public float dirX;
        public float dirY;
        public uint casterID;
        public bool isActive;
        public float startLocationX;
        public float startLocationY;
        public float currentLocationX;
        public float currentLocationY;
        public int ID;
    }

    public List<ActiveSpellData> GetActiveSpellData() {
        List<ActiveSpellData> temp = new List<ActiveSpellData>();
        foreach(GameObject s in allActiveSpells) {
            SpellRecords sr = s.GetComponent<SpellRecords>();
            ActiveSpellData a = new ActiveSpellData();
            a.dirX = sr.dir.x;
            a.dirY = sr.dir.y;
            a.casterID = sr.caster.GetComponent<CharacterInfo>().id;
            a.isActive = sr.isActive;
            a.startLocationX = sr.StartLocation.x;
            a.startLocationY = sr.StartLocation.y;
            a.currentLocationX = s.transform.position.x;
            a.currentLocationY = s.transform.position.y;
            a.ID = sr.spell.ID;
            temp.Add(a);
        }
        return temp;
    }

    public void LoadActiveSpellData(List<ActiveSpellData> asd) {
        foreach(GameObject c in new List<GameObject>(allActiveSpells)) {
            allActiveSpells.Remove(c);
            Destroy(c);
        }
        foreach(ActiveSpellData a in asd) {
            CastSpell(GetSpellFromID(a.ID), new Vector3(a.startLocationX, a.startLocationY, 0), new Vector3(a.currentLocationX, a.currentLocationY, 0), new Vector3(a.dirX, a.dirY, 0), NPCManagerScript.ins.GetNPCInSceneFromID(a.casterID));
        }
    }


}
