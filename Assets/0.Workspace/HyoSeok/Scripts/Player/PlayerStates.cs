using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    bool isMove;
    bool isAttack;
    bool isDie;
    bool isMovingAttack;
    bool isExcalibur;
    bool isSkill;
    bool isUsedItem;
    public bool IsMove { get { return isMove; } }
    public bool IsAttack { get { return isAttack;} set { isAttack = value; } }
    public bool IsDie { get { return isDie; } }
    public bool IsMovingAttack { get { return isMovingAttack; } }
    public bool IsExcalibur { get { return isExcalibur; } }
    public bool IsSkill { get { return isSkill; } }
    public bool IsUsedItem { get { return isUsedItem; } set { isUsedItem = value; } }
    private void Start()
    {
        isMove = true;
        isAttack = true;
        isDie = false;
        isMovingAttack = true;
        isExcalibur = false;
        isSkill = true;
        isUsedItem = true;

    }
    // 움직이지 못하는 상황일때
    public void Stop()
    {
        if (!isDie)
        {
            if (!isExcalibur)
            {
                isAttack = false;
            }
            isMove = false;
        }
    }
    public void MoveAttack()
    {
        if(!isDie)
        {
            if (!isExcalibur)
            {
                isAttack = false;
            }

        }
    }
    public void AttackNomal()
    {
        if (!isDie)
        {
            if (!isExcalibur)
            {
                isAttack = true;
            }
            isMove = true;
        }
    }
    // 움직이지 못하는 상황에서 벗어날때
    public void Normal()
    {
        if (!isDie)
        {
            if (!isExcalibur)
            {
                isAttack = true;
            }
            isMove = true;
            isSkill = true;
        }
    }
    
    // 죽었을때 죽는거 빼고 다 false로 바꿔줘야됨
    public void DieStates()
    {
        isDie = true;
        isAttack = false;
        isSkill = false;
        isMove = false;
        isMovingAttack = false;
        isExcalibur = false;
    }

    // 죽고 살아날때
    public void ReviveState()
    {
        Start();
    }

    // 엑스칼리버 장착하거나 버려질때 
    public void IsExcaliburEquipped(bool OnOff)
    {
        if (!isDie)
        {
            isExcalibur = OnOff;
            isAttack = !OnOff;
            isMovingAttack = !OnOff;
            isSkill = !OnOff;
        }
    }
    public void BlackOutState()
    {
        isSkill = false;
    }
}
