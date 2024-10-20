using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : BaseState
{
    private float searchTimer;
    private float moveTimer;
    private Vector3 randomPos;
    private Vector3 target;
    public override void ResetAttack(){}

    public override void Enter()
    {
        //eğer bir enemy bile playeri görüyorsa diğerleri playerin bulunduğu konuma gider eğer player kaybolduysa rastgele konumlarda player aranır
        if(stateMachine.agentControl.anybodySee){
            enemy.Agent.SetDestination(stateMachine.agentControl.LastKnowPos);
            stateMachine.agentControl.anybodySee = false;
            enemy.animator.SetBool("isWalking", true);

        }
        
        //direkt bu kodu kullanmış olsaydık uzakta olan enemy sürekli rastgele konum üretmiş olup acık sapıtırdı 
        //hepsi aynı konumda sıkışmasın diye rastgele konumlarda player aranır
        else
        {
            randomPos = Random.insideUnitSphere * 10 ;
            enemy.Agent.SetDestination(stateMachine.agentControl.LastKnowPos + randomPos);
            //enemy'nin o anki hedefi(hedef sürekli değişeceği için tek bir değişkene atıp daha rahat  kontrol etmek için oluşturdum)
            target = stateMachine.agentControl.LastKnowPos + randomPos;
            enemy.animator.SetBool("isWalking", true);
        }
    }
        public override void Perform()
    {
        //if(enemy.CanSeePlayer()){
        //    stateMachine.ChangesState(new AttackState());
        //}

        if(Vector3.Distance(enemy.Agent.transform.position, target) < 10.0f/*enemy.Agent.remainingDistance < enemy.Agent.stoppingDistance*/){
            searchTimer += Time.deltaTime;
            moveTimer += Time.deltaTime;
            if(moveTimer > Random.Range(3,5)){
                randomPos = Random.insideUnitSphere * 10;
                enemy.Agent.SetDestination(stateMachine.agentControl.LastKnowPos + randomPos);
                target = stateMachine.agentControl.LastKnowPos + randomPos;
                moveTimer = 0;
            }
            if(searchTimer > 10){
                stateMachine.ChangesState(new PatrolState());
            }
        }
    }
        public override void Exit()
    {
        
    }
}
