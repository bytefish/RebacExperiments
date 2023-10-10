﻿CREATE PROCEDURE [Application].[usp_TemporalTables_ReactivateTemporalTables]
AS BEGIN

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[UserTask]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[UserTask]'

		ALTER TABLE [Application].[UserTask] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[UserTask] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[UserTaskHistory], DATA_CONSISTENCY_CHECK = ON));
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[UserTaskPriority]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[UserTaskPriority]'

		ALTER TABLE [Application].[UserTaskPriority] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[UserTaskPriority] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[UserTaskPriorityHistory], DATA_CONSISTENCY_CHECK = ON));
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[UserTaskStatus]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[UserTaskStatus]'

		ALTER TABLE [Application].[UserTaskStatus] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[UserTaskStatus] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[UserTaskStatusHistory], DATA_CONSISTENCY_CHECK = ON));
	END
    
END