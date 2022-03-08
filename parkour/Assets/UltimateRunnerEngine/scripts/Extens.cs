using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public static class Extens
{
  // 超框显示...
  public static void Ellipsis(this Text textComp, string value)
  {
      if (string.IsNullOrEmpty(value))
      {
          textComp.text = "";
          return;
      }
      RectTransform rectTransform = textComp.GetComponent<RectTransform>();
      Font font = textComp.font;
      font.RequestCharactersInTexture(value, textComp.fontSize, FontStyle.Normal);
      CharacterInfo characterInfo;
      font.GetCharacterInfo('.', out characterInfo, textComp.fontSize);
      float width = characterInfo.advance * 3;
      string str = "";
      for (int i = 0; i < value.Length; i++)
      {
          font.GetCharacterInfo(value[i], out characterInfo, textComp.fontSize);
          width += characterInfo.advance;
          if (width > rectTransform.rect.width)
          {
              break;
          }
          str += value[i];
      }
      if (width > rectTransform.rect.width)
      {
          textComp.text = str + "...";
      }
      else
      {
          textComp.text = value;
      }
  }

}
