using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemLootSystem
{
    //Feature: 
    // - 페이즈별 드랍테이블 관리,생성
    // - 
    /*

    비밀방 페이즈별 상자 드랍확률 : https://docs.google.com/document/d/1BMvRrFPT0V70asCxQ-p7PAhUqTIAFj9MMvDMKvgeUPc/edit 참조
     */

    public enum TreasureRarity
    {
        D = 0,
        C = 1,
        B = 2,
        A = 3,
        S = 4
    }

    public class DropTableManager : Singleton<DropTableManager>
    {
        /*
        enum GamePhase
        {
            Phase1 = 1, // 0 sec ~ 1 min 59 sec // Monster Droptable phase 1
            Phase2 = 2, // 2 min ~ 3 min 59 sec // Monster Droptable Phase 2 , 비밀방 2개 
            Phase3 = 3, // 4 min ~ 5 mim 59 sec // Monster Droptable Phase 2 , 비밀방 2개 
            Phase4 = 4, // 6 min ~ 6 min 59 sec // Monster Droptable phase 4 , 비밀방 1개 
            Phase5 = 5 //  7 min ~              // Monster Droptable phase 5        
        }*/

        // 드랍 확률 저장된 SO
        [SerializeField] DropTableRate dropTableRate;

        //D ~ S 등급까지의 보물상자 넣어둘것
        [SerializeField] List<TreasureBox> treasureBoxs = new List<TreasureBox>();

        //몬스터용 드랍테이블 넣어둘것
        //[SerializeField] List<ItemDropTable> normalMonsterDropTables = new List<ItemDropTable>();
        //[SerializeField] List<ItemDropTable> eliteMonsterDropTables = new List<ItemDropTable>();
        public ItemCollection itemCollection;
        [SerializeField] ItemDropTable normalMonsterDropTable;
        [SerializeField] ItemDropTable eliteMonsterDropTable;

        // 임시
        GamePhase curPhase = GamePhase.Phase1;

        //페이즈에 따라 적절한 드랍테이블 반환 ,비밀방 생성시 호출 // tlqkf하드코딩...
        public TreasureBox GetTresureBoxSecretRoom()
        {
            if (treasureBoxs.Count == 0)
                return null;

            curPhase = Manager.Game.Phase;
            float randValue = Random.Range(0, 100);
            switch (curPhase)
            {
                case GamePhase.Phase2:
                    if (randValue <= dropTableRate.Phase2AtreasureBoxRate)
                    {
                        return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.A);
                    }
                    else
                    {
                        return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.S);
                    }
                case GamePhase.Phase3:
                    if (randValue <= dropTableRate.Phase3AtreasureBoxRate)
                    {
                        return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.A);
                    }
                    else
                    {
                        return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.S);
                    }
                case GamePhase.Phase4:
                    return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.S);
                default:
                    return null;
            }
        }

        public TreasureBox GetTreasureBoxNormalRoom()
        {
            float value = Random.Range(0, 100);
            float currentRate = 0;

            currentRate += dropTableRate.StreasureBoxRate;
            if (value < currentRate) //  S
            {
                return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.S);
            }

            currentRate += dropTableRate.AtreasureBoxRate;
            if (value < currentRate) //  A
            {
                return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.A);
            }

            currentRate += dropTableRate.BtreasureBoxRate;
            if (value < currentRate) //  B
            {
                return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.B);
            }

            currentRate += dropTableRate.CtreasureBoxRate;
            if (value < currentRate) //  C
            {
                return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.C);
            }

            // Default 
            return treasureBoxs?.First(tb => tb.rarity == TreasureRarity.D);
        }

        /// <summary>
        /// 페이즈에 맞는 몬스터 드랍테이블 반환
        /// </summary>
        /// <param name="type"> 노말 몬스터인지, 엘리트 몬스터인지, 교체할 것</param>
        /// <returns></returns>
        public ItemDropTable GetItemDropTable(MonsterType type)
        {
            curPhase = Manager.Game.Phase;

            if (type == (MonsterType) 0)
                return GetNormalMonsterDropTable(curPhase);
            else
                return GetEliteMonsterDropTable(curPhase);
        }

        ItemDropTable GetNormalMonsterDropTable(GamePhase curPhase)
        {
            return normalMonsterDropTable;
        }

        ItemDropTable GetEliteMonsterDropTable(GamePhase curPhase)
        {
            return eliteMonsterDropTable;
        }

    }
}