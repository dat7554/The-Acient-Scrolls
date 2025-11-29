=== collectAncientScrollQuestStart ===
{ CollectAncientScrollQuestState :
    - "RequirementsNotMet": -> requirementsNotMet
    - "CanStart": -> canStart
    - "InProgress": -> inProgress
    - "CanFinish": -> canFinish
    - "Finished": -> finished
    - else: -> END
}

= requirementsNotMet
-> END

= canStart
Adventurer, I need your help… could you enter the dungeon and retrieve an Ancient Scroll hidden deep within its chambers? It is said to contain powerful knowledge that could protect human society from dark forces. I’ll reward you handsomely for your courage.
* [Yes]
    Please be careful and return safely with the scroll. Its secrets are vital to all of humanity.
    ~ StartQuest(CollectAncientScrollQuestId)
* [No]
    I understand… it’s a dangerous task. But if you change your mind, remember: the safety of human society depends on that scroll.
- ->END

= inProgress
-> END

= canFinish
You’ve returned! Were you able to retrieve the Ancient Scroll?
* [Yes]
    Marvelous! You’ve done a great service. Humanity owes you its gratitude. Here is your reward for such bravery.
    ~ FinishQuest(CollectAncientScrollQuestId)
* [No]
    
- ->END

= finished
Thank you for helping me!
-> END