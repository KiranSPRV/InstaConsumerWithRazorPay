package crc6495f8d705b5cfc639;


public class FirebaseService
	extends com.google.firebase.iid.FirebaseInstanceIdService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onTokenRefresh:()V:GetOnTokenRefreshHandler\n" +
			"";
		mono.android.Runtime.register ("ParkHyderabad.Droid.FirebaseService, ParkHyderabad.Android", FirebaseService.class, __md_methods);
	}


	public FirebaseService ()
	{
		super ();
		if (getClass () == FirebaseService.class)
			mono.android.TypeManager.Activate ("ParkHyderabad.Droid.FirebaseService, ParkHyderabad.Android", "", this, new java.lang.Object[] {  });
	}


	public void onTokenRefresh ()
	{
		n_onTokenRefresh ();
	}

	private native void n_onTokenRefresh ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
