/*===============================================================================
Copyright (c) 2024 PTC Inc. and/or Its Subsidiary Companies. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using Vuforia;

public class MTAdvancedObserverEventHandler : MTExtendedObserverEventHandler
{
    ModelTargetsManager mModelTargetsManager;
    bool mIsModelTargetReferenceStored;

    protected override void Start()
    {
        base.Start();

        mModelTargetsManager = FindFirstObjectByType<ModelTargetsManager>();
    }

    void Update()
    {
        if (mIsModelTargetReferenceStored || mObserverBehaviour == null) 
            return;
        
        var modelTarget = mObserverBehaviour as ModelTargetBehaviour;

        if (modelTarget != null && mModelTargetsManager != null)
        {
            mModelTargetsManager.AddAdvancedModelTarget(modelTarget);
            mIsModelTargetReferenceStored = true;
        }
    }
    
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        mModelTargetsManager.EnableSymbolicTargetsUI(false);
    }
}