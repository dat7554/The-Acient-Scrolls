=== collectApplesQuestStart ===
{ CollectApplesQuestState :
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
Hey… um, could you help me collect 5 apples? I need them for my mom.
* [Yes]
    Really? Thanks! I’ll wait here.
    ~ StartQuest(CollectApplesQuestId)
* [No]
    Oh… okay. If you change your mind, please come back later.
- ->END

= inProgress
-> END

= canFinish
Oh! You’re back. Did you finish collecting the apples?
* [Yes]
    Thank you! Here, take these coins as a reward!
    ~ FinishQuest(CollectApplesQuestId)
* [No]
    
- ->END

= finished
Thank you for helping me!
-> END