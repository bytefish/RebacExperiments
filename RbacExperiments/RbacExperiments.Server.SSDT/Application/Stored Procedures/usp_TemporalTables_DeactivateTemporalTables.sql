﻿CREATE PROCEDURE [Application].[usp_TemporalTables_DeactivateTemporalTables]
AS BEGIN
	IF OBJECTPROPERTY(OBJECT_ID('[Application].[UserTask]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[UserTask]'

		ALTER TABLE [Application].[UserTask] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[UserTask] DROP PERIOD FOR SYSTEM_TIME;
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[UserTaskPriority]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[UserTaskPriority]'

		ALTER TABLE [Application].[UserTaskPriority] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[UserTaskPriority] DROP PERIOD FOR SYSTEM_TIME;
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[UserTaskStatus]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[UserTaskStatus]'

		ALTER TABLE [Application].[UserTaskStatus] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[UserTaskStatus] DROP PERIOD FOR SYSTEM_TIME;
	END

END