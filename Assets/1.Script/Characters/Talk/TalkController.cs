using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITalkable
{
    NPCController Controller { get; set; }
    TalkData CurrentTalkData { get; set; }
    string Name { get; set; }
    string Text { get; set; }

    // ��ȭ�� �� �ʿ��� ������ ����
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
        // �ؽ�Ʈ�� �ϼ����� �ʾ��� �� ���� ���� �� �ڵ� �Ѿ�� ���
        if (!UIManager.instance.dialogueManager.IsAutoText())
        {
            // ��ġ Ȥ�� ��ŵ
            yield return null;
            yield return new WaitUntil(() => UIManager.instance.dialogueManager.IsNextDialogueRequested() || UIManager.instance.dialogueManager.IsSkipRequested());
        }
        else
        {
            // ���� ���
            yield return new WaitUntil(() => !UIManager.instance.dialogueManager.IsTyping());

            // ���� ��尡 �����Ǿ����� Ȯ��
            if (!UIManager.instance.dialogueManager.IsAutoText())
            {
                // ���� ��尡 �����Ǿ��� ��, ����ڰ� ���� ��糪 ��ŵ�� ��û�� ������ ���
                yield return new WaitUntil(() => UIManager.instance.dialogueManager.IsNextDialogueRequested() || UIManager.instance.dialogueManager.IsSkipRequested());
            }
            else
            {
                // ���� ��尡 �����Ǵ� ���, ���� �ð� ��� �� ���� ���� �̵�
                yield return new WaitForSeconds(1.5f);
            }
        }
    }
}
