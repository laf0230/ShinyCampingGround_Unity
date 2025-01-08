using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITalkable
{
    NPCController Controller { get; set; }
    TalkData CurrentTalkData { get; set; }
    string Name { get; set; }
    string Text { get; set; }

    // 대화할 때 필요한 데이터 세터
    public void SetTalkData(TalkData data);

    public IEnumerator Talk();
}

public class TalkController
{
    public NPCController Controller { get; set; }
    public TalkData CurrentTalkData { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public int nextID { get; set; }

    public TalkController(NPCController controller)
    {
        Controller = controller;
    }

    public virtual IEnumerator Talk()
    {
        yield return null;
    }

    public void SetTalkData(TalkData data)
    {
        Name = data.npcName;
        Debug.Log(data.stringID);
        Text = GameManager.Instance.dataManager.GetScriptData(data.stringID).text;
        nextID = data.nextScriptID; 
    }

    public int GetNextID()
    {
        return nextID;
    }
}

public class GeneralTalk : TalkController, ITalkable
{
    public GeneralTalk(NPCController controller) : base(controller)
    {
        Controller = controller;
    }

    public override IEnumerator Talk()
    {
        yield return null;
        Controller.speechBubbleController.SetName(Name);
        Controller.speechBubbleController.SetText(Text);
        Controller.speechBubbleController.FlickBubble();
    }
}

public class SpecialTalk : TalkController, ITalkable
{
    public SpecialTalk(NPCController controller) : base(controller)
    {
        Controller = controller;
    }

    public override IEnumerator Talk()
    {
        Controller.cam.Priority = 11;

        if (Name == null)
        {
            Debug.Log(CurrentTalkData.npcName + "Character Name is null");
        }

        UIManager.instance.dialogueManager.ActiveDialogue(Name, Text);
        // 텍스트가 완성되지 않았을 때 오토 해제 시 자동 넘어가기 취소
        if (!UIManager.instance.dialogueManager.IsAutoText())
        {
            // 터치 혹은 스킵
            yield return null;
            yield return new WaitUntil(() => UIManager.instance.dialogueManager.IsNextDialogueRequested() || UIManager.instance.dialogueManager.IsSkipRequested());
        }
        else
        {
            // 오토 모드
            yield return new WaitUntil(() => !UIManager.instance.dialogueManager.IsTyping());

            // 오토 모드가 해제되었는지 확인
            if (!UIManager.instance.dialogueManager.IsAutoText())
            {
                // 오토 모드가 해제되었을 때, 사용자가 다음 대사나 스킵을 요청할 때까지 대기
                yield return new WaitUntil(() => UIManager.instance.dialogueManager.IsNextDialogueRequested() || UIManager.instance.dialogueManager.IsSkipRequested());
            }
            else
            {
                // 오토 모드가 유지되는 경우, 일정 시간 대기 후 다음 대사로 이동
                yield return new WaitForSeconds(1.5f);
            }
        }
    }
}
