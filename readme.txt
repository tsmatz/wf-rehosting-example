/// This demo is for :

  Workflow rehosting for end-user and compile / deploy.

  Project:
  CustomActivityLibrary
    This is custom activities using in rehosting workflow sample.
  CustomWorkflowLibrary
    This is workflow library for WCF service.
  CustomWorkflowConsole
    This launches service.
  ClientApplication
    This is client Windows application using the service.
  RehostingSample
    This can modify and deploy the workflow in CustomWorkflowLibrary.

/// How to install

1. Modify service endpoint address in app.config at ClientApplication.

You can add tasks using button in client application,
and drag / drop these activities.
Then select activity, right-click, and select delete,
so you can delete these tasks.

Add activity adds 3.
Multi activity mutiplies 3.
