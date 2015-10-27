using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using N;
using N.UI.Tools;

namespace N.UI {

  /// Property inspection binder
  [CustomEditor(typeof(PropertyBinding))]
  public class PropertyBindingInspector : Editor {

    private EditorSelectComponent selectTargetComponent;
    private EditorSelectProperty selectTargetProperty;
    private EditorSelectComponent selectSourceComponent;
    private EditorSelectProperty selectSourceProperty;

    public void OnEnable() {
      var root = (PropertyBinding) target;
      selectTargetComponent = new EditorSelectComponent("Target component", root.info.targetComponent);
      selectTargetProperty = new EditorSelectProperty("Target property", root.info.targetProperty);
      selectSourceComponent = new EditorSelectComponent("Source component", root.info.sourceComponent);
      selectSourceProperty = new EditorSelectProperty("Source property", root.info.sourceProperty);

      // Set initial state
      selectTargetComponent.bind(root.gameObject);
      selectTargetProperty.bind(selectTargetComponent.component);
      if (root.source != null) {
        selectSourceComponent.bind(root.source);
        selectSourceProperty.bind(selectSourceComponent.component);
      }
    }

    public override void OnInspectorGUI() {
      var root = (PropertyBinding) target;
      base.OnInspectorGUI();

      EditorGUILayout.Space();

      // Allow user to select the component on the parent of this object
      // to pick a property from.
      EditorGUILayout.BeginHorizontal();
      selectTargetComponent.bind(root.gameObject);
      var new_target_component = selectTargetComponent.update();
      EditorGUILayout.EndHorizontal();

      // If the component changed, repopulate the property list.
      EditorGUILayout.BeginHorizontal();
      if (new_target_component) {
        N.Console.Log(selectTargetComponent.component);
        selectTargetProperty.bind(selectTargetComponent.component);
      }
      var new_target_property = selectTargetProperty.update();
      EditorGUILayout.EndHorizontal();

      // Assign properties back to the root
      if (new_target_component) { root.info.targetComponent = selectTargetComponent.value; }
      if (new_target_property) { root.info.targetProperty = selectTargetProperty.value; }

      // Allow user to select the component on the source object, if set.
      if (root.source != null) {
        EditorGUILayout.BeginHorizontal();
        selectSourceComponent.bind(root.source);
        var new_source_component = selectSourceComponent.update();
        EditorGUILayout.EndHorizontal();

        // If the component changed, repopulate the property list.
        EditorGUILayout.BeginHorizontal();
        if (new_source_component) { selectSourceProperty.bind(selectSourceComponent.component); }
        var new_source_property = selectSourceProperty.update();
        EditorGUILayout.EndHorizontal();

        // If a new property was selected, update the root binding
        if (new_source_component) { root.info.sourceComponent = selectSourceComponent.value; }
        if (new_source_property) { root.info.sourceProperty = selectSourceProperty.value; }

      }
    }
  }
}
