EXTERNAL StartQuest(questID)
EXTERNAL AdvanceQuest(questID)
EXTERNAL FinishQuest(questID)

VAR CollectApplesQuestId = "CollectApplesQuest"
VAR CollectApplesQuestState = "RequirementsNotMet"

VAR CollectAncientScrollQuestId = "CollectAncientScrollQuest"
VAR CollectAncientScrollQuestState = "RequirementsNotMet"

// ink files
INCLUDE collect_apples_quest_npc.ink
INCLUDE collect_ancient_scroll_quest_npc.ink