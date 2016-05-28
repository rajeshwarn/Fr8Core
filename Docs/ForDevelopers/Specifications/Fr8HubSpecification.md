# FR8 HUB SPECIFICATION

## Hub Endpoints

### ActivitiesController
----------------
#### **Path:**	*/activities/create*  
**Type:**	*POST*  
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
actionTemplateId |	Guid	|  |  |		id of the activity template of the activity instance that will be created   
label |	string |		null |  |	Label that will be shown on the header   
order |	int |	true |	null |  |	Hierarchical order of the activity according to parent-children relation   
parentNodeId |	Guid |	true |	null | If it is a child activity, that is the parent activity id of the instance that will be created   
authorizationTokenId |	Guid |	true |	null |	 To get authorization tokens for the outside systems such as slack, docusign etc..  

**Return Values:**	ActivityDTO  
**Description:**	Creates an instance of activity from activityTemplates, and provides necessary authorization to use them.  

#### **Path:**	*/activities/configure*  
**Type:**	POST  
**Input Parameters:**

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
curActionDesignDTO | 	ActivityDTO			| | |   
**Return Values:**	ActivityDTO   
**Description:**	Callers to this endpoint expect to receive back what they need to know to encode user configuration data into the Action. the typical scenario involves a front-end client  calling this and receiving back the same Action they passed, but with an attached Configuration Crate. The client renders UI based on the Configuration Crate, collects user inputs, and saves them as values in the Configuration Crate json. The updated Configuration Crate is then saved to the server so it will be available to the processing Terminal at run-time.   
#### **Path:**	*/activities/get*
**Type:**	*GET*   
**Input Parameters:**

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
 id |	Guid	| | |		 id of the demanded activity   
**Return Values:**	ActivityDTO   
**Description:**	 Simple getter for activity.  
 
#### **Path:**	*/activities/delete*   
**Type:**	*DELETE*   
**Input Parameters:**

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
 id |	Guid	| | |	 id of the activity to delete.
confirmed |	boolean |		false | |	Deleting an activity can cause effects on downstream activities. When this parameter is false, the downstream activities are being checked, and if there is an effect on them, the user will be informed by a message. If user confirms, then the activity is directly deleted.   

**Return Values:**	void   
**Description:**		 To delete an activity   

#### **Path:**	*/activities/deleteActivity*   
**Type:** *DELETE*   
**Input Parameters:**

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
 id |	Guid	| | |		 id of the activity to delete  
 
**Return Values:**	void     
**Description:**			This endpoint for terminals to delete activity, since there is no user interaction when a request is sent from terminal, there will be no confirm message needed too.  

#### **Path:**	*/activities/deleteChildNodes*  
**Type:** *DELETE*  
**Input Parameters:**

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
 activityId |	Guid | | |			 id of the activity to delete  
 
**Return Values:**	void    
**Description:**		Remove all child Nodes and clear activity values  

#### **Path:**	*/activities/save*  
**Type:**	*POST*  
**Input Parameters:**  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
 curActionDTO |	ActivityDTO	| | |		Current object that will be saved or updated   
**Return Values:**	ActivityDTO  
**Description:**	Saves or updates the given action  
 
 ### ActivityTemplatesController
------------------------------------
#### **Path:**	*/activityTemplates/get*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
id | Guid | False | --- | Id of the activity template to retrieve   
**Return Values**	ActivityTemplateDTO   
**Description:**	Simple getter for activity template 
 
### AlarmsController
-----------------------------------------
#### **Path:**	*/alarms/post*  
**Type:**	POST    
**Input Parameters:**   

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
 alarmDTO |	 AlarmDTO		| | |	  
**Return Values:**	void   
**Description:**		Alarms provide ability to add some delay between activities. This endpoints set the start time of the first activity that is going to be executed after delay.   
#### **Path:**	*/alarms/executeTerminalWithLogging*
**Type:**	*POST*    
**Input Parameters:**   

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
 alarmDTO |	 AlarmDTO		| | |	  
**Return Values:**	void   
**Description:**		Alarms provide ability to add some delay between activities. This endpoints set the start time of the first activity that is going to be executed after delay.  

### AuthenticationController
-----------------------------------
#### **Path:**	*/authentication/authenticate*
**Type:**	*POST*    
**Input Parameters:**   

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
credentials |	CredentialsDTO	| | |		Credentials of the user to login   
**Return Values:**	IHttpActionResult   
**Description:**	Gets the user credentials an provides necessary authentication. Returns authorazition token, terminal id and error message if there is any.   

#### **Path:**	*/authentication/getOAuthInitiationURL*
**Type:**		*GET*   
**Input Parameters:**   

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
terminalId |	[FromUri(Name = bidb)] int		| | |

**Return Values:**	string   
**Description:** none

#### **Path:**	*/authentication/getAuthToken*  
**Type:**		*GET*   
**Input Parameters:**   

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
curFr8UserId |	[FromUri]string			| | |
externalAccountId |	[FromUri]string			| | |
terminalId |	[FromUri] string			| | |

**Return Values:**	AuthorizationTokenDO    
**Description:** none

### ConfigurationController
---------------------------------------
#### **Path:**	*/configuration/getAppInsightsInstrKey*  
**Type:**	*GET*  
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
**Return Values:**	string   
**Description:**

### ContainersController

#### **Path:**	*/containers/getPayload*
**Type:**	*GET*  
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid		|	| | Container id   
**Return Values:**	PayloadDTO   
**Description:**	Gets the payload of the container given.  
 
#### **Path:**	*/containers/getIdsByName*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
name |	string			| | |
**Return Values:**	Json   
**Description:** none   

#### **Path:**	*/containers/get*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
name |	string |	 true	| null	| Container id   
**Return Values:**	Json  
**Description:**	 Return the Containers accordingly to ID given   

### CriteriaController

#### **Path:**	*/criteria/bySubPlanId*   
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid	 | | |		 subPlanId   
**Return Values:**	CriteriaDTO   
**Description:**	Retrieve criteria by Subroute.Id   

#### **Path:**	*/criteria/update*
**Type:**	*PUT*  
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
dto |	CriteriaDTO	| | |		 CriteriaDTO to update    
**Return Values:**	CriteriaDTO  
**Description:**	Recieve criteria with global id, update criteria, and return updated criteria.  

### DocumentationController
-----------------------------------
#### **Path:**	*/documentation/activity*   
**Type:**	*POST*   
**Input Parameters:**   

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
 curActivityDTO |	 ActivityDTO [FromBody]			| | |   
**Return Values:**		SolutionPageDTO or List<SolutionPageDTO>   
**Description:**	This endpoint returns help menu of the current activity, if the activity that is passed to function is not a solution. If a solution is given as a parameter, then the endpoint returns the documentation page of the solution.   

### EventsController
-----------------------------------
#### **Path:**	*/events*
**Type:**	*POST*
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
raw |	CrateDTO			| | |  
**Return Values**	IHttpActionResult   
**Description:**	Called when a Terminal wants to report an event to the Hub. The Hub expects a single EventReport crate. It will open the crate and inspect the ExternalAccountId and ExternalDomainId properties.
If ExternalAccountId value equals to "system1@fr8.co" Hub will expect one or many LoggingData crates in EventReport crate storage, which are processed and handled as Facts or Incidents.
Else Hub searches AuthorizationTokens that match ExternalAccountnId and/or ExternalDomainId. For each token that it finds, it searches for Plans that use that token, and launches plans that have an event subscription crate matching one of EventReport.EventNames values. This allows Terminals to log data and Plans to subscribe to specific events, and get notified and launched when they arrive.
### FactsController
-------------------------------------
#### **Path:**	*/facts/processQuery*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
query |	FactDO			| | |   
**Return Values:**	HistoryItemDTO  
**Description:** none  

### FieldController
-----------------------------------------
#### **Path:**	*/field/exists*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
fieldCheckList |	List<FieldValidationDTO> | | |			Field list to check  
**Return Values:**	List<FieldValidationResult>  
**Description:**	Checks whether fields in the fieldList are exists or not  

### FilesController

#### **Path:**	*/files*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
*Return Values:*	FileDO   
*Description:*	 Uploads the file content to Azure Blob storage and then saves the file object to the Fr8 database.  

#### **Path:**	*/files/details*  
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	int	| | |		 id of the file object   
**Return Values:**	FileDTO   
**Description:**	Takes the id of the file and then returns the FileDTO object.   

#### **Path:**	*/files/list*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
**Return Values:**	IList<FileDTO>   
**Description:**	Lists all files for the current user stored on Fr8   

#### **Path:**	*/files*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id | 	int	|		| | id of the file object  
**Return Values:**	byte[]  
**Description:**	Downloads user's given file ID as byte array.

#### **Path:**	*/files*
**Type:**	*DELETE*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id | 	int	|		| | id of the file object  
**Return Values:**	void  
**Description:**	Deletes the file in Fr8 database and Azure Blob Storage using the given File ID.  

### ManageAuthTokenController
-------------------------------------------
#### **Path:**	*/manageAuthToken/get*  
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
**Return Values:**	List<ManageAuthToken_Terminal>   
**Description:**	Extract userbs auth-tokens and parent terminals.   

#### **Path:**	*/manageAuthToken/revoke*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid | | |			 id of the token to revoke  
**Return Values:**	IHttpActionResult     
**Description:**	Revoke token.    

#### **Path:**	*/manageAuthToken/terminalsByActivities*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
actionIds |	IEnumerable<Guid>		 | | |	
**Return Values:**	List<ManageAuthToken_Terminal_Activity>   
**Description:**	Takes the activity ids and manages the necessary auth tokens for them than returns the authenticated terminal activity list.

#### **Path:**	*/manageAuthToken/apply*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
apply |	IEnumerable<ManageAuthToken_Apply>		| | |  	  
**Return Values:**	IHttpActionResult    
**Description:**	Applies the authentication. If the token set as main, it will be also set as default token.  

#### **Path:**	*/manageAuthToken/setDefault*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid	| | |		 id of the auth token   
**Return Values:**	IHttpActionResult   
**Description:**	Takes the id of the authentication token and sets it as default authentication token.  

### ManifestRegistryController
------------------------------------
#### **Path:**	*/manifestRegistry/*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---     
**Return Values:**	IEnumerable<ManifestDescriptionCM>  
**Description:** 	returns list of  manifest descriptions from MultiTenantObjectRepository

#### **Path:**	*/manifestRegistry/*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
description |	ManifestDescriptionDTO			| | |    
**Return Values:**	ManifestDescriptionDTO     
**Description:**	Takes the manifest description and saves to db.   

#### **Path:**	*/manifestRegistry/query*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
data |	ManifestRegistryParams			| | Contains name and version string fields |

**Return Values**	(dynamic)    
**Description:** if ManifestRegistryParams.version is empty: gets the manifest description with given name from last version.
 	otherwise: check that if there is any manifest description with given name in given version.   



### ManifestsController
---------------------------------
#### **Path:**	*/manifests/get*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	int	| | | 		id of the manifest   
**Return Values**	CrateDTO   
**Description:**	get the Manifest with given id.   

### NotificationsController
-------------------------------------
#### **Path:**	*/notifications/post* 
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
notificationMessage |	TerminalNotificationDTO			| | |
**Return Values**	IHttpActionResult   
**Description:**	Takes the message, checks whether it is a terminal call or client call, and sets channel accordingly. Then notifies the selected channel.

### OrganizationController
------------------------------------
#### **Path:**	*/organization/get*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id	| int	| | |		 id of the organization
**Return Values**	OrganizationDTO   
**Description:**	Takes the id and returns the Organization   
#### **Path:**	*/organization/put*
**Type:**	*PUT*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
dto |	OrganizationDTO			| | |
**Return Values**	OrganizationDTO   
**Description:**	Updates organization   

### PlanNodesController
--------------------------------------
#### **Path:**	*/planNodesController/get*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid		| | |	
**Return Values**	ActivityTemplateDTO   
**Description:**	Returns the activity template with given id.   
#### **Path:**	*/planNodesController/getUpstreamActivities
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid		| | |  	
**Return Values**	List<PlanNodeDO>   
**Description:**	Returns the Upstream Activities of the activity with given id. 

#### **Path:**	*/planNodesController/getDownstreamActivities*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid			| | |    
**Return Values**	List<PlanNodeDO>   
**Description:**	 Returns the Downstream Activities of the activity with given id.
 

#### **Path:**	*/planNodesController/getAvailableActivities*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   

**Return Values**	IEnumerable<ActivityTemplateCategoryDTO>     
**Description:**	Returns the activities with status active.   

#### **Path:**	*/planNodesController/getAvailableData*
**Type:**	*GET*    
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid			| | | 
direction |	CrateDirection | |		CrateDirection.Upstream 	|
availability |	AvailabilityType | | AvailabilityType.RunTime	|   
**Return Values**	IncomingCratesDTO   
**Description:**	Gets available data from upstream for the activity with given id.   
#### **Path:**	*/planNodesController/getAvailableActivities*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
tag |	string	| | |  		
**Return Values**	IEnumerable<ActivityTemplateDTO>    
**Description:**	Gets the available activities with given tag.   

### PlansController
------------------------------------
#### **Path:**	*/plans/post*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
planDto |	PlanEmptyDTO			| | |
updateRegistrations |	bool | |		false	|
**Return Values**	PlanDTO   
**Description:**	Creates and saves the plan.  
#### **Path:**	*/plans/getFullPlan*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id | 	Guid			| | |
**Return Values**	PlanDTO  
**Description:**	Returns the plan with given id   

#### **Path:**	*/plans/getByActivity*
**Type:**	*GET*
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid			| | |
**Return Values**	PlanDTO  
**Description:**	Returns the plan with given activity id.

#### **Path:**	*/plans/getByQuery*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
planQuery|	[FromUri] PlanQueryDTO			| | | 
**Return Values**	PlanResultDTO   
**Description:**	Returns the PlanResult with given query  

#### **Path:**	*/plans/getByName*
**Type:**	*GET*     
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
name |	string			| | |
visibility |	PlanVisibility | |		PlanVisibility.Standard	|  
**Return Values**	List<PlanDTO>   
**Description:**	Returns the list of planDTObs with given name and visibility.  

#### **Path:**	*/plans/copy*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid			| | |
name |	string			| | |
**Return Values**	dynamic ? ? ?   
**Description:**	none   
#### **Path:**	*/plans/get* 
**Type:**	*GET*  
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid |	true |	null	|  
**Return Values**	PlanEmptyDTO   
**Description:**	none   

#### **Path:**	*/plans/createSolution*  
**Type:**	*POST*  
**Input Parameters:**

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---  
solutionName	| string	| | |		Name of the solution template of the solution that will be created   

**Return Values:**	PlanDTO    
**Description:**	Creates an instance of solution from solutionTemplates, and configures it. 

#### **Path:**	*/plans/putActivity*
**Type:**	*POST*
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   

**Return Values**	  
**Description:**	  

#### **Path:**	*/plans/delete*
**Type:**	*DELETE*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id	| Guid			| | | 
**Return Values**	Guid   
**Description:**	Deletes the plan with given id   
#### **Path:**	*/plans/activate*   
**Type:**	*POST*
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
planId |	Guid			| | |
planBuilderActivate |	bool |		false	| | 
**Return Values**	ActivateActivitiesDTO   
**Description:**	Activates the plan and generates the notifier.  

#### **Path:**	*/plans/deactivate*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
planId | 	Guid			| | | 
**Return Values**	string  
**Description:**	Deactivates the plan with given id, returns the string result: bsuccessb or bno actionb   

#### **Path:**	*/plans/createFindObjectsPlan*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   

**Return Values**	dynamic {id = plan.Id}    
**Description:**	Creates Find Object Plan.

#### **Path:**	*/plans/run*
**Type:**	*GET*
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
planId |	Guid			| | |
containerId |	Guid |	true |	null	|
**Return Values**	ContainerDTO   
**Description:**	Method for plan execution continuation from URL  

#### **Path:**	*/plans/run*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   

planId |	Guid			|||
model |	[FromBody]PayloadVM			|||
containerId |	Guid |	true |	null	|
**Return Values**	ContainerDTO   
**Description:**	Runs the plan with given id, payload and container.   

#### **Path:**	*/plans/runWithPayload*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
planId |	Guid			| | |
payload |	[FromBody]List<CrateDTO> |	true |	null |
**Return Values**	ContainerDTO   
**Description:**	Run the plan with given id and payload.  

### ReportController
----------------------------------
#### **Path:**	*/report/getIncidentsByQuery*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
historyQueryDTO |	[FromUri] HistoryQueryDTO			|||
**Return Values**	HistoryResultDTO<IncidentDTO>   
**Description:**	Gets incidents with given history result query   
#### **Path:**	*/report/getFactsByQuery*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
historyQueryDTO |	[FromUri] HistoryQueryDTO			|||
**Return Values**	HistoryResultDTO<FactDTO>   
**Description:**	Gets facts with given history result query   

### SubPlansController
--------------------------------
#### **Path:**	*/subPlans/post*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
subPlanDTO |	SubPlanDTO			| | | 
**Return Values**	SubPlanDTO   
**Description:**	Creates and saves given subPlan   
#### **Path:**	*/subPlans/put*
**Type:**	*PUT*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
subPlanDTO |	SubPlanDTO	|||		
**Return Values**	SubPlanDTO   
**Description:**	Updates given subPlan   

#### **Path:**	*/subPlans/delete*
**Type:**	*DELETE*  
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid		|||	
**Return Values**	SubPlanDTO   
**Description:**	Deletes given subPlan   

#### **Path:**	*/subPlans/firstActivity*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	Guid			|||
**Return Values**	ActivityDTO   
**Description:**	Gets the first activity in parent hierarchy.   

### TerminalsController
------------------------------------
#### **Path:**	*/terminals/get*
**Type:**	*GET*
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   

**Return Values**	List of 
TerminalDTO   
{
  "Name": null,
  "Label": null,
  "Version": null,
  "TerminalStatus": 0,
  "Endpoint": null,
  "Description": null,
  "AuthenticationType": 0
}
**Description:**	Returns list of terminals for current user   

#### **Path:**	*/terminals/post*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
terminalDto |	TerminalDTO			|||
**Return Values**	
TerminalDTO   
{
  "Name": null,
  "Label": null,
  "Version": null,
  "TerminalStatus": 0,
  "Endpoint": null,
  "Description": null,
  "AuthenticationType": 0
}
**Description:**	Creates and saves terminal object.   

### UsersController
------------------------------------

#### **Path:**	*/users*
**Type:**	*GET*   
**Input Parameters:** 

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
 |		|||

**Return Values**	 List of 
UserDTO  
{
  "Id": null,
  "FirstName": null,
  "LastName": null,
  "UserName": null,
  "EmailAddressID": 0,
  "EmailAddress": null,
  "Status": 0,
  "Role": null,
  "organizationId": null,
  "ProfileId": "00000000-0000-0000-0000-000000000000",
  "Class": null
}
**Description:**	Returns all available users in the form of List of UserDTO

#### **Path:**	*/users/userdata/id*
**Type:**	*GET*   
**Input Parameters:**  Id

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
id |	string	|||Id of the user

**Return Values**	 
UserDTO  
{
  "Id": null,
  "FirstName": null,
  "LastName": null,
  "UserName": null,
  "EmailAddressID": 0,
  "EmailAddress": null,
  "Status": 0,
  "Role": null,
  "organizationId": null,
  "ProfileId": "00000000-0000-0000-0000-000000000000",
  "Class": null
}
**Description:**	If Id is not null or empty, returns the UserDTO of the given user ID. Otherwise returns the currently logged in user.

#### **Path:**	*/users/update*
**Type:**	*POST*
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
oldPassword |	string			| | |Old Password of the user
newPassword |	string	|||		New Password of the user

**Return Values**	IHttpActionResult   
**Description:**	Updates user password

### WarehouseController
------------------------------------
#### **Path:**	*/warehouse/post*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
string |	userId			|||
crates |	List<CrateDTO>			|||
**Return Values**	List<CrateDTO>
**Description:**	

### WebServicesController
------------------------------------
#### **Path:**	*/webServices/get*
**Type:**	*GET*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
**Return Values**	List<WebServiceDTO>   
**Description:**	Returns the collection of all active web services 
#### **Path:**	*/webServices/post*   
**Type:**	*POST*    
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
webService |	WebServiceDTO			|||
**Return Values**	WebServiceDTO   
**Description:**	none   
#### **Path:**	*/webServices/getActivities*
**Type:**	*POST*   
**Input Parameters:**  	  

Name |	Type |	Nullable	| Default |	Description   
--- | --- | --- | --- | ---   
categories |	ActivityCategory[]			|||
**Return Values**	WebServiceActivitySetDTO   
**Description:**	none