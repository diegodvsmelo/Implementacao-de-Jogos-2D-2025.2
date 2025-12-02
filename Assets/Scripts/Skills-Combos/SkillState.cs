[System.Serializable]
public class SkillState
{
    public SkillData data;      
    public int currentCharges;  
    public float lastUsedTime;  


    public SkillState(SkillData data)
    {
        this.data = data;
        this.currentCharges = data.maxCharges; 
        this.lastUsedTime = -100f; 
    }

    public void ResetCharges()
    {
        currentCharges = data.maxCharges;
    }
}