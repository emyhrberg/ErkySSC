using ErkySSC.Core.Debug;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ErkySSC.Core.Configs.ConfigElements;

public class CustomDictionaryElement : DictionaryElement
{
    // This method is copied from vanilla implementation, except we add a Item icon and a text for each entry in the dictionary.
    protected override void SetupList()
    {
        Log.Debug("SetupList() called from CDE");

        base.DataList.Clear();
        int top = 0;
        this.dataWrapperList = new List<IDictionaryElementWrapper>();
        Type genericType = typeof(DictionaryElementWrapper<,>).MakeGenericType(this.keyType, this.valueType);
        if (base.Data == null)
        {
            return;
        }
        ICollection keys = ((IDictionary)base.Data).Keys;
        ICollection values = ((IDictionary)base.Data).Values;
        IEnumerator keysEnumerator = keys.GetEnumerator();
        IEnumerator valuesEnumerator = values.GetEnumerator();
        int i = 0;
        while (keysEnumerator.MoveNext())
        {
            valuesEnumerator.MoveNext();
            IDictionaryElementWrapper proxy = (IDictionaryElementWrapper)Activator.CreateInstance(genericType, keysEnumerator.Current, valuesEnumerator.Current, (IDictionary)base.Data);
            this.dataWrapperList.Add(proxy);
            _ = base.MemberInfo.Type.GetGenericArguments()[0];
            PropertyFieldWrapper wrappermemberInfo = ConfigManager.GetFieldsAndProperties(this).ToList()[0];
            Tuple<UIElement, UIElement> tuple = UIModConfig.WrapIt(base.DataList, ref top, wrappermemberInfo, this, 0, this.dataWrapperList, genericType, i);
            tuple.Item2.Left.Pixels += 24f;
            tuple.Item2.Width.Pixels -= 24f;

            // --- Add our text! ---
            object keyObj = keysEnumerator.Current;
            int displayIndex = i + 1;

            if (tuple.Item2 is ConfigElement configElement)
            {
                configElement.TextDisplayFunction = () => $"{displayIndex}: {FormatKeyWithItemTag(keyObj)}";
            }

            UIModConfigHoverImage deleteButton = new UIModConfigHoverImage(base.DeleteTexture, Language.GetTextValue("tModLoader.ModConfigRemove"))
            {
                VAlign = 0.5f
            };
            object o = keysEnumerator.Current;
            deleteButton.OnLeftClick += delegate
            {
                ((IDictionary)base.Data).Remove(o);
                this.SetupList();
                Interface.modConfig.SetPendingChanges();
            };
            tuple.Item1.Append(deleteButton);
            i++;
        }
    }

    // Helper to add text to each dictionary key element.
    private static string FormatKeyWithItemTag(object keyObj)
    {
        if (keyObj == null)
            return "null";

        if (keyObj is ItemDefinition itemDef)
        {
            // Only show the tag if we have a valid loaded type.
            if (itemDef.Type > 0)
                return $"[i:{itemDef.Type}] {Lang.GetItemNameValue(itemDef.Type)}";

            // Fallback for unloaded/invalid definitions.
            return itemDef.ToString();
        }

        return keyObj.ToString();
    }

}