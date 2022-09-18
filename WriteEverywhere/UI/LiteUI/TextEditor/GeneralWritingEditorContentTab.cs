﻿using Kwytto.LiteUI;
using Kwytto.UI;
using Kwytto.Utils;
using System;
using System.Linq;
using UnityEngine;
using WriteEverywhere.Localization;
using WriteEverywhere.Xml;

namespace WriteEverywhere.UI
{
    internal class GeneralWritingEditorContentTab : WTSBaseParamsTab<BoardTextDescriptorGeneralXml>
    {
        public override Texture TabIcon { get; } = KResourceLoader.LoadTextureKwytto(CommonsSpriteNames.K45_DiskDrive);

        private static readonly TextContent[] m_contents = new[] { TextContent.ParameterizedText, TextContent.ParameterizedSpriteSingle, TextContent.ParameterizedSpriteFolder, TextContent.TextParameterSequence };
        private static readonly string[] m_optionsContent = m_contents.Select(x => x.ValueToI18n()).ToArray();

        private readonly GUIRootWindowBase m_root;
        private readonly Func<PrefabInfo> infoGetter;

        public GeneralWritingEditorContentTab(GUIColorPicker colorPicker, Func<PrefabInfo> infoGetter)
        {
            m_root = colorPicker.GetComponentInParent<GUIRootWindowBase>();
            this.infoGetter = infoGetter;
        }

        protected override void DrawListing(Vector2 tabAreaSize, BoardTextDescriptorGeneralXml currentItem)
        {
            GUILayout.Label($"<i>{Str.WTS_TEXT_CONTENTVALUE_TAB}</i>");
            var item = currentItem;
            bool isEditable = true;
            GUIKwyttoCommons.AddComboBox(tabAreaSize.x, "K45_WTS_TEXT_CONTENT", ref item.textContent, m_optionsContent, m_contents, m_root, isEditable);
            switch (item.textContent)
            {

                case TextContent.ParameterizedText:
                case TextContent.ParameterizedSpriteFolder:
                case TextContent.ParameterizedSpriteSingle:
                    var param = item.DefaultParameterValue;
                    GUIKwyttoCommons.AddButtonSelector(tabAreaSize.x, Str.WTS_CONTENT_TEXTVALUE, param is null ? GUIKwyttoCommons.v_null : param.IsEmpty ? GUIKwyttoCommons.v_empty : param.ToString(), () => OnGoToPicker(currentItem, -1), isEditable);
                    break;
                case TextContent.TextParameterSequence:
                    if (item.ParameterSequence is null)
                    {
                        item.ParameterSequence = new TextParameterSequence(new[] { new TextParameterSequenceItem("NEW", Rendering.TextRenderingClass.Vehicle) }, Rendering.TextRenderingClass.Vehicle);
                    }
                    var paramSeq = item.ParameterSequence;
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("#", GUILayout.Width(25));
                        GUILayout.Label(Str.WTS_CONTENT_TEXTVALUE);
                        GUILayout.Label(Str.WTS_PARAMSEQ_STEPLENGTH, GUILayout.Width(100));
                        GUILayout.Label(Str.WTS_PARAMSEQ_ACTIONS, GUILayout.Width(60));
                        GUILayout.Space(30);
                        var rect = GUILayoutUtility.GetLastRect();
                        rect.height = 18;
                        if (isEditable && GUI.Button(rect, "+"))
                        {
                            AddItem(paramSeq);
                        }
                    }
                    var line = 0;
                    foreach (var seqItem in paramSeq)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label((line + 1).ToString(), GUILayout.Width(25));
                            GUIKwyttoCommons.AddButtonSelector(seqItem?.Value?.ToString(), () => OnGoToPicker(currentItem, line), isEditable);
                            if (isEditable)
                            {
                                int newLenght;
                                if ((newLenght = GUIIntField.IntField("K45_WTS_PARAMSEQ_STEPLENGTH_" + line, (int)seqItem.m_length, 0)) != seqItem.m_length)
                                {
                                    paramSeq.SetLengthAt(line, newLenght);
                                }
                            }
                            else
                            {
                                GUILayout.Label(seqItem.m_length.ToString("#,##0") + "Fr.", GUILayout.Width(25));
                            }
                            GUILayout.Space(30);
                            var rect = GUILayoutUtility.GetLastRect();
                            rect.height = 18;
                            if (isEditable && line > 0 && GUI.Button(rect, "↑"))
                            {
                                MoveUp(paramSeq, line);
                            }
                            GUILayout.Space(30);
                            rect = GUILayoutUtility.GetLastRect();
                            rect.height = 18;
                            if (isEditable && line < paramSeq.TotalItems - 1 && GUI.Button(rect, "↓"))
                            {
                                MoveDown(paramSeq, line);
                            }
                            GUILayout.Space(30);
                            rect = GUILayoutUtility.GetLastRect();
                            rect.height = 18;
                            if (isEditable && line > 0 && GUI.Button(rect, "X"))
                            {
                                RemoveAt(paramSeq, line);
                            }
                        }
                        line++;
                    }
                    break;

            }
        }

        private void MoveUp(TextParameterSequence paramSeq, int line) => paramSeq.MoveUp(line);
        private void MoveDown(TextParameterSequence paramSeq, int line) => paramSeq.MoveDown(line);
        private void RemoveAt(TextParameterSequence paramSeq, int line) => paramSeq.RemoveAt(line);
        private void AddItem(TextParameterSequence paramSeq) => paramSeq.Add(new TextParameterWrapper(), 250);
        private void OnGoToPicker(BoardTextDescriptorGeneralXml currentItem, int key)
        {
            TextParameterWrapper value;
            if (key == -1)
            {
                value = currentItem.DefaultParameterValue ?? new TextParameterWrapper();
            }
            else if (currentItem.ParameterSequence is TextParameterSequence tps && key >= 0 && key < tps.TotalItems)
            {
                value = currentItem.ParameterSequence.ElementAt(key).Value;
            }
            else
            {
                return;
            }
            GoToPicker(key, currentItem.textContent, value, currentItem);
        }

        protected override string GetAssetName(BoardTextDescriptorGeneralXml item) => infoGetter()?.name;
        protected override void SetTextParameter(BoardTextDescriptorGeneralXml item, int currentEditingParam, string paramValue)
        {
            if (currentEditingParam == -1)
            {
                item.SetDefaultParameterValueAsString(paramValue, Rendering.TextRenderingClass.Vehicle);
            }
            else if (item.ParameterSequence is TextParameterSequence tps && currentEditingParam >= 0 && currentEditingParam < tps.TotalItems)
            {
                tps.SetTextAt(currentEditingParam, paramValue, Rendering.TextRenderingClass.Vehicle);
            }
        }
    }
}