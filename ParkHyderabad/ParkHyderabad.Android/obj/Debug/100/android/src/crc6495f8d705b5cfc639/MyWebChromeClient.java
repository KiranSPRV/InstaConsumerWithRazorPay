package crc6495f8d705b5cfc639;


public class MyWebChromeClient
	extends android.webkit.WebChromeClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onProgressChanged:(Landroid/webkit/WebView;I)V:GetOnProgressChanged_Landroid_webkit_WebView_IHandler\n" +
			"";
		mono.android.Runtime.register ("ParkHyderabad.Droid.MyWebChromeClient, ParkHyderabad.Android", MyWebChromeClient.class, __md_methods);
	}


	public MyWebChromeClient ()
	{
		super ();
		if (getClass () == MyWebChromeClient.class)
			mono.android.TypeManager.Activate ("ParkHyderabad.Droid.MyWebChromeClient, ParkHyderabad.Android", "", this, new java.lang.Object[] {  });
	}


	public void onProgressChanged (android.webkit.WebView p0, int p1)
	{
		n_onProgressChanged (p0, p1);
	}

	private native void n_onProgressChanged (android.webkit.WebView p0, int p1);

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
