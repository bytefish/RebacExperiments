PRINT 'Inserting [Identity].[RelationTuple] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Identity].[RelationTuple]
-----------------------------------------------
MERGE INTO [Identity].[RelationTuple] AS [Target]
USING (VALUES 
      (1, 'UserTask', 323, 'owner', 'User',   2, NULL, 1, @ValidFrom, @ValidTo)
     ,(2, 'UserTask', 323, 'viewer', 'Organization',   1, 'member', 1, @ValidFrom, @ValidTo)
     ,(3, 'UserTask', 152, 'viewer', 'Organization',   2, 'member', 1, @ValidFrom, @ValidTo)
     ,(4, 'UserTask', 152, 'viewer', 'Organization',   1, 'member', 1, @ValidFrom, @ValidTo)
     ,(5, 'Organization',  1,   'member', 'User',  2, NULL, 1, @ValidFrom, @ValidTo)
     ,(6, 'Organization',  1,   'member', 'User',  3, NULL, 1, @ValidFrom, @ValidTo)
     ,(7, 'Organization',  2,   'member', 'User',  4, NULL, 1, @ValidFrom, @ValidTo)
) AS [Source]
(
     [RelationTupleID] 
    ,[ObjectNamespace] 
    ,[ObjectKey]       
    ,[ObjectRelation]  
    ,[SubjectNamespace]
    ,[SubjectKey]      
    ,[SubjectRelation] 
    ,[LastEditedBy]    
    ,[ValidFrom]       
    ,[ValidTo]         
)
ON (
    [Target].[RelationTupleID] = [Source].[RelationTupleID]
)
WHEN NOT MATCHED BY TARGET THEN
    INSERT 
        (
             [RelationTupleID]
            ,[ObjectNamespace]
            ,[ObjectKey]
            ,[ObjectRelation]
            ,[SubjectNamespace]
            ,[SubjectKey]
            ,[SubjectRelation]
            ,[LastEditedBy]
            ,[ValidFrom]
            ,[ValidTo]
        )
    VALUES 
        (
             [Source].[RelationTupleID]
            ,[Source].[ObjectNamespace]
            ,[Source].[ObjectKey]
            ,[Source].[ObjectRelation]
            ,[Source].[SubjectNamespace]
            ,[Source].[SubjectKey]
            ,[Source].[SubjectRelation]
            ,[Source].[LastEditedBy]
            ,[Source].[ValidFrom]
            ,[Source].[ValidTo]
        );