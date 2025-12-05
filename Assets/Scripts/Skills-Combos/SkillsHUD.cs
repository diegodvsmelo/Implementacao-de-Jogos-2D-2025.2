using UnityEngine;
using UnityEngine.UI;

public class SkillHUD : MonoBehaviour
{
    [Header("Conexões")]
    public SkillQueueManager skillManager;

    [Header("Slots Ativos (Q, W, E, R)")]
    public SkillSlotUI[] activeSlots; 

    [Header("Próxima Skill (Fila)")]
    public SkillSlotUI nextSkillSlot; 
    public Image nextSkillBorder; // A borda da imagem pequena

    void Start()
    {
        // Busca automática do Player se não estiver assignado
        if (skillManager == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if(player != null) skillManager = player.GetComponent<SkillQueueManager>();
        }
    }

    void Update()
    {
        if (skillManager == null) return;

        // 1. Atualiza Slots Ativos (Q, W, E, R)
        for (int i = 0; i < activeSlots.Length; i++)
        {
            if (i < skillManager.activeSlots.Length)
            {
                activeSlots[i].UpdateSlot(skillManager.activeSlots[i]);
            }
        }

        // 2. Atualiza a "Próxima Skill" da Fila
        UpdateNextSkillUI();
    }

    void UpdateNextSkillUI()
    {
        if (nextSkillSlot == null) return;

        // Verifica se existe alguém na fila de espera
        if (skillManager.waitingQueue.Count > 0)
        {
            // Peek() olha quem é o PRIMEIRO da fila (Posição 4 no GDD)
            // Essa é a skill que vai entrar assim que você gastar uma atual.
            SkillState nextSkill = skillManager.waitingQueue.Peek();
            
            // Atualiza o ícone e textos do slot pequeno
            nextSkillSlot.UpdateSlot(nextSkill);
            
            // GARANTIA: Cor Sólida (Sem transparência)
            if(nextSkillBorder) nextSkillBorder.color = Color.white;
        }
        else
        {
            // Se a fila estiver vazia (menos de 5 skills aprendidas)
            nextSkillSlot.UpdateSlot(null);
            
            // Mantém a borda visível e branca (ou da cor original), sem ficar transparente
            if(nextSkillBorder) nextSkillBorder.color = Color.white;
        }
    }
}