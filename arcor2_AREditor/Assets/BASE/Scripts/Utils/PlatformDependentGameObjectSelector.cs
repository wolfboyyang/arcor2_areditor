using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDependentGameObjectSelector : MonoBehaviour {

    [Tooltip("GameObjects specified in this list will be enabled only when running on Android platform. On every other platform, these objects will be disabled.")]
    public List<GameObject> GameObjectsForANDROIDOnly = new List<GameObject>();

    [Tooltip("GameObjects specified in this list will be enabled only when running on iOS platform. On every other platform, these objects will be disabled.")]
    public List<GameObject> GameObjectsForIOSOnly = new List<GameObject>();

    [Tooltip("GameObjects specified in this list will be enabled only when running on Windows, Linux or Mac OS X platform. On every other platform, these objects will be disabled.")]
    public List<GameObject> GameObjectsForSTANDALONEOnly = new List<GameObject>();

    private void Awake() {
// disable ARSession and enable again does not reset the arsession and will not work properly
#if UNITY_ANDROID && AR_ON
        foreach (GameObject obj in GameObjectsForIOSOnly.Except(GameObjectsForANDROIDOnly)) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameObjectsForSTANDALONEOnly.Except(GameObjectsForANDROIDOnly)) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameObjectsForANDROIDOnly) {
            obj.SetActive(true);
        }
#elif UNITY_IOS && AR_ON
        foreach (GameObject obj in GameObjectsForANDROIDOnly.Except(GameObjectsForIOSOnly)) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameObjectsForSTANDALONEOnly.Except(GameObjectsForIOSOnly)) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameObjectsForIOSOnly) {
            obj.SetActive(true);
        }
#elif UNITY_STANDALONE || !AR_ON
        foreach (GameObject obj in GameObjectsForANDROIDOnly) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameObjectsForIOSOnly) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameObjectsForSTANDALONEOnly) {
            obj.SetActive(true);
        }
#else
        foreach (GameObject obj in GameObjectsForANDROIDOnly) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameObjectsForIOSOnly) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameObjectsForSTANDALONEOnly) {
            obj.SetActive(false);
        }
#endif
    }

}
