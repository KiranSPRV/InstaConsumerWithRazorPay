using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Widget;
using ParkHyderabad.Droid;
using ParkHyderabad.Helper;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMaterialEntry), typeof(CustomMaterialEntryRenderer))]
namespace ParkHyderabad.Droid
{
    public class CustomMaterialEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null || e.NewElement == null) return;

            if (e.NewElement is CustomMaterialEntry customEntry)
            {
                EditText.SetHighlightColor(color: customEntry.MyHighlightColor.ToAndroid());

                try
                {
                    JNIEnv.SetField(EditText.Handle, JNIEnv.GetFieldID(JNIEnv.FindClass(typeof(TextView)), "mCursorDrawableRes", "I"), 0);
                    using (var textViewTemplate = new TextView(EditText.Context))
                    {
                        var field = textViewTemplate.Class.GetDeclaredField("mEditor");
                        field.Accessible = true;
                        var editor = field.Get(EditText);

                        string[]
                        fieldsNames = { "mTextSelectHandleLeftRes", "mTextSelectHandleRightRes", "mTextSelectHandleRes" },
                        drawablesNames = { "mSelectHandleLeft", "mSelectHandleRight", "mSelectHandleCenter" };

                        for (int index = 0; index < fieldsNames.Length && index < drawablesNames.Length; index++)
                        {
                            field = textViewTemplate.Class.GetDeclaredField(fieldsNames[index]);
                            field.Accessible = true;
                            Drawable handleDrawable = ContextCompat.GetDrawable(Context, field.GetInt(EditText));

                            handleDrawable.SetColorFilter(new PorterDuffColorFilter(customEntry.MyHighlightColor.ToAndroid(), PorterDuff.Mode.SrcIn));

                            field = editor.Class.GetDeclaredField(drawablesNames[index]);
                            field.Accessible = true;
                            field.Set(editor, handleDrawable);
                        }
                    }
                }
                catch (NoSuchFieldError)
                {
                }
                catch (NoSuchFieldException)
                {
                }
                catch (ReflectiveOperationException)
                {
                }
            }
        }
    }
}