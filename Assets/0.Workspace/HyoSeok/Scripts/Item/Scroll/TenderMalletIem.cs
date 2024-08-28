using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TenderMalletIem", menuName = "Item/TenderMalletIem")]
public class TenderMalletIem :BaseScrollItem
{
    TenderMallet Mallet;
    protected override void UseScroll(GameObject user)
    {
        // 보운이 작품
        /*int leehyoseok = 10;
        int yeeyerin = 5;
        leehyoseok += leehyoseok;
        leehyoseok = 10 > yeeyerin ? 10 < yeeyerin ? yeeyerin > leehyoseok ? 1 < 2 ? 1 : leehyoseok : 1 : 1 : leehyoseok;
        */
        Mallet = user.GetComponentInChildren<TenderMallet>();
        if (Mallet == null)
            return;
        Mallet.UseItem();
    }
   /* // 민교작품
    * //여기서부터 개쩌는 코드
    abcd == ㄱㄴㄷㄹ;
        안녕하세요 == hello;
        #include <iostream>
        
        int main()
        //여기까지가 개쩌는 코드*/
}
