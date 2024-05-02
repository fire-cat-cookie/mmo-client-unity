using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabNavigation : MonoBehaviour {

    public List<TMP_InputField> elements;

    void Start() {
        if (elements.Count > 0) {
            elements[0].Select();
        }
    }

    private TMP_InputField GetCurrentlySelected() {
        foreach (TMP_InputField element in elements) {
            if (element.isFocused) {
                return element;
            }
        }
        return null;
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab)) {

            TMP_InputField selected = GetCurrentlySelected();
            int nextIndex;
            if (selected != null && elements.Contains(selected)) {
                nextIndex = elements.IndexOf(selected) + 1;
            } else {
                nextIndex = 0;
            }

            if (elements.Count > nextIndex) {
                elements[nextIndex].Select();
            }
        }
    }
}
