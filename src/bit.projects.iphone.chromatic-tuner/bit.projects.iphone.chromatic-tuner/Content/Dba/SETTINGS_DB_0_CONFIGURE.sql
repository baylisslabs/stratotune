
CREATE TABLE IF NOT EXISTS 'User_Settings'(
'Id' integer primary key autoincrement not null ,
'Name' text not null,
'DisallowDelete' integer not null,
'A4Calibration' float not null,
'Notation_Id' text not null,
'Transposition_Id' text not null,
'Temperament_Id' text not null,
'NeedleDamping_Id' text not null,
'FrequencyDisplay' integer not null,
'PitchPipeWaveform_Id' text not null);

CREATE UNIQUE INDEX IF NOT EXISTS 'User_Settings_Name' on 'User_Settings'('Name');

CREATE TABLE IF NOT EXISTS 'System_Config'(
'Id' integer primary key not null ,
'CurrentUserSetting_Id' integer not null,
'DefaultUserSetting_Id' integer not null,
'HeadPhoneAlertDisabled' integer not null,
FOREIGN KEY('CurrentUserSetting_Id') REFERENCES 'User_Settings'('Id'),
FOREIGN KEY('DefaultUserSetting_Id') REFERENCES 'User_Settings'('Id')
);

CREATE TABLE IF NOT EXISTS 'App_Feedback'(
'Id' integer primary key not null ,
'VersionAtLastUsage' text,
'VersionFirstUsedTimeStamp' text,
'UsesCount' integer not null,
'SignificantUsesCount' integer not null,
'DoNotRemindBeforeTime' text,
'RatingFlowCompleted' integer not null
);

INSERT OR IGNORE INTO 'User_Settings'(
    'Id', 
    'Name' ,
    'DisallowDelete',
    'A4Calibration',
    'Notation_Id',
    'Transposition_Id',
    'Temperament_Id',
    'NeedleDamping_Id',
    'FrequencyDisplay', 
    'PitchPipeWaveform_Id' )
VALUES (
    1, 
    'Default',
    1,
    440,
    'English',
    'C',
    'EqualTemperament',
    'Low',
    0, 
    'Sine' );

INSERT OR IGNORE INTO 'System_Config'('Id','CurrentUserSetting_Id','DefaultUserSetting_Id','HeadPhoneAlertDisabled') VALUES (1,1,1,0);

INSERT OR IGNORE INTO 'App_Feedback'(
	'Id',
	'VersionAtLastUsage',
	'VersionFirstUsedTimeStamp',
	'UsesCount',
	'SignificantUsesCount',
	'DoNotRemindBeforeTime',
	'RatingFlowCompleted' )
 VALUES (
	 1,
	 null,
	 null,
	 0,
	 0,
	 null,
	 1 );
 