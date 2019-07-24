using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHUDLog : MonoBehaviour {

    public RectTransform logPrefab;
    public RectTransform gridlayout;
    public GameObject pathPrefab;
    public List<Movement> loggedMoves = new List<Movement>();
    private int count = -1;
    private bool isEmpty;
    private List<int> randomHashList = new List<int>();

    public bool IsEmpty {
        get {
            return isEmpty;
        }
    }


    public class Move {
        public MoveType mt;
        public string msg;
        public enum MoveType {
            Movement,
            Attack
        }
    }
    public class Movement : Move {
        public Vector3[] destination;
        public GameObject path;
        public LineRenderer pathLR;
        public int hash;
        public int queuePos;
        void Start() {
            mt = MoveType.Movement;
        }
        public void SetDest(Vector3[] des) {
            destination = des;
            msg = "Move to position at X: " + destination[destination.Length - 1].x + " and Y: " + destination[destination.Length - 1].y + ".";
        }
    }

    public void ChangeNode(Vector3 o, Vector3 n) {

        foreach(Movement m in loggedMoves) {
            if(m.destination[m.destination.Length - 1] == o) {
                GameManagerScript.ins.playerInfo.polyNav.map.FindPath(m.destination[0], n, ChangeNodeLevelsBeg);
                m.pathLR.SetPositions(m.destination);
                m.path.transform.position = n;
            }
            if(m.destination[0] == o) {
                GameManagerScript.ins.playerInfo.polyNav.map.FindPath(n, m.destination[m.destination.Length - 1], ChangeNodeLevelsEnd);
                //m.pathLR.positionCount = m.destination.Length;
                m.pathLR.SetPositions(m.destination);
                break;
            }
        }
        
    }

    void ChangeNodeLevelsBeg(Vector2[] path) {
        Vector3[] a = toVectorThree(path);

        foreach(Movement m in loggedMoves) {
            if (m.destination[0] == a[0]) {
                m.pathLR.positionCount = a.Length;
                m.destination = a;
            }
        }
    }

    void ChangeNodeLevelsEnd(Vector2[] path) {
        Vector3[] a = toVectorThree(path);
        foreach (Movement m in loggedMoves) {
            if (m.destination[m.destination.Length - 1] == a[a.Length - 1]) {
                m.pathLR.positionCount = a.Length;
                m.destination = a;
            }
        }
    }

    private Vector3[] toVectorThree(Vector2[] path) {
        Vector3[] a = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++) {
            a[i] = new Vector3(path[i].x, path[i].y, 0);
        }
        return a;
    }

    public void InsertNode(Vector3 n, int startIndex, int endIndex, Vector3[] d, int dHash) {
        List<Movement> endofListMovement = new List<Movement>();
        int index = 0;
        Movement targetMovement = new Movement();
        //get index
        foreach(Movement m in loggedMoves) {
            if(m.hash == dHash) {
                index = loggedMoves.IndexOf(m);
                targetMovement = m;
            }
        }
        //remove everything after index
        List<Movement> tempCopy = new List<Movement>(loggedMoves);
        foreach (Movement m in tempCopy) {
            if(tempCopy.IndexOf(m) > index) {
                loggedMoves.Remove(m);
                Destroy(m.path);
                Destroy(m.pathLR);
                endofListMovement.Add(m);
            }
        }
        //add new logged moves
        loggedMoves.Remove(targetMovement);
        Destroy(targetMovement.path);
        Destroy(targetMovement.pathLR);
        if (!HasPosition()) {
            CombatManager.ins.combatDrawMovePosition.playerAgent.map.FindPath(GameManagerScript.ins.player.transform.position, n, LogMovePosition);
        } else {
            CombatManager.ins.combatDrawMovePosition.playerAgent.map.FindPath(GetLastPosition(), n, LogMovePosition);
        }
        CombatManager.ins.combatDrawMovePosition.playerAgent.map.FindPath(GetLastPosition(), d[d.Length - 1], LogMovePosition);

        foreach(Movement m in endofListMovement) {
            LogMovePosition(m.destination);
        }

    }

    public void UpdateLineFromMovement() {

        GameManagerScript.ins.playerInfo.polyNav.map.FindPath(GameManagerScript.ins.GetPlayerFeetPosition(), loggedMoves[0].destination[loggedMoves[0].destination.Length - 1], DrawLineFromMovement);
    }

    public Vector3 GetDestination() {
        return loggedMoves[0].destination[loggedMoves[0].destination.Length - 1];
    }

    void DrawLineFromMovement(Vector2[] des) {
        if(des == null) {
            return;
        }
        Vector3[] temp = new Vector3[des.Length];
        for(int i = 0; i <des.Length; i++) {
            temp[i] = new Vector3(des[i].x, des[i].y, 0);
        }

        loggedMoves[0].SetDest(temp);
        loggedMoves[0].path.transform.position = temp[temp.Length - 1];
        loggedMoves[0].pathLR = loggedMoves[0].path.GetComponent<LineRenderer>();
        loggedMoves[0].pathLR.positionCount = temp.Length;
        loggedMoves[0].pathLR.SetPositions(temp);
    }

    public bool HasReachedPosition() {
        if(loggedMoves.Count != 0) {
            //GameManagerScript.ins.player.transform.position != loggedMoves[0].destination[loggedMoves[0].destination.Length - 1]
            if (Vector3.Distance(GameManagerScript.ins.GetPlayerFeetPosition(), loggedMoves[0].destination[loggedMoves[0].destination.Length - 1]) >= 0.2f) {
                return false;
            } else {
                RemoveFirstLogMovementPosition();
                return HasReachedPosition();
            }
        }
        return true;
    }

    public bool CanReachFirstPosition(Vector3 o, Vector3 p) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(o, p - o, Vector3.Distance(o, p), CombatManager.ins.wallTest);
        if (hits.Length % 2 == 0) {
            return true;
        }
        return false;
    }

	
	public bool HasPosition() {
        if(loggedMoves.Count > 0) {
            return true;
        }
        return false;
    }

    public Vector3 GetLastPosition() {
        Vector3 temp = Vector3.zero;
        foreach(Movement m in loggedMoves) {
            temp = m.destination[m.destination.Length - 1];
        }

        return temp;
    }

    public Vector3[] LoggedPath() {
        List<Vector3> temp = new List<Vector3>();
        foreach(Movement m in loggedMoves) {
            foreach (Vector3 p in m.destination) {
                temp.Add(p);
            }
        }
        Vector3[] returnArray = new Vector3[temp.Count];
        for(int i = 0; i < temp.Count; i++) {
            returnArray[i] = temp[i];
        }
        return returnArray;
    }

    public void LogMovePosition(Vector3[] des) {
        Vector2[] temp = new Vector2[des.Length];
        int x = 0;
        foreach(Vector3 v in des) {
            temp[x] = v.xy();
            x++;
        }
        LogMovePosition(temp);
    }

    public void LogMovePosition(Vector2[] des) {
        List<Vector3> temp = new List<Vector3>();
        foreach(Vector2 p in des) {
            temp.Add(new Vector3(p.x, p.y, 0));
        }
      
        Movement mvmt = new Movement();
        mvmt.SetDest(temp.ToArray());
        mvmt.path = Instantiate(pathPrefab);
        mvmt.path.transform.position = temp[temp.Count - 1];
        mvmt.pathLR = mvmt.path.GetComponent<LineRenderer>();
        mvmt.pathLR.startWidth = 0.05f;
        mvmt.pathLR.endWidth = 0.05f;
        mvmt.pathLR.positionCount = temp.Count;
        mvmt.pathLR.SetPositions(temp.ToArray());

        int tempHash = Random.Range(0, 999999);
        while (randomHashList.Contains(tempHash)) {
            tempHash = Random.Range(0, 999999);
        }
        randomHashList.Add(tempHash);
        mvmt.hash = tempHash;

        loggedMoves.Add(mvmt);
        mvmt.queuePos = loggedMoves.IndexOf(mvmt);
        isEmpty = false;
    }

    public void RemoveLastLogMovementPosition() {
        Movement mvmt = null;
        foreach(Movement m in loggedMoves) {
            mvmt = m;
        }
        if(mvmt != null) {
            loggedMoves.Remove(mvmt);
            Destroy(mvmt.path.gameObject);
        }
        if(loggedMoves.Count == 0) {
            isEmpty = true;
        } else {
            isEmpty = false;
        }
    }

    public void RemoveAllMovement() {
        foreach(Movement m in loggedMoves) {
            Destroy(m.path.gameObject);
        }
        loggedMoves.Clear();
        isEmpty = true;
    }

    public void RemoveFirstLogMovementPosition() {
        Destroy(loggedMoves[0].path.gameObject);
        loggedMoves.RemoveAt(0);
        if (loggedMoves.Count == 0) {
            isEmpty = true;
        } else {
            isEmpty = false;
        }
    }
}
