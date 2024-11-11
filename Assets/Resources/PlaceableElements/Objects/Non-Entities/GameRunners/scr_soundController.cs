using System.Collections;
using UnityEngine;

public class scr_soundController : MonoBehaviour
{
    scr_gameController currGameController;
    public static event GLOBAL_VARS.controllerResponseSignal initialGamePrimedSignal;
    public static event GLOBAL_VARS.controllerResponseSignal menuPrimedSignal;
    public static event GLOBAL_VARS.controllerResponseSignal menuEndSignal;
    public void InstantiateMusicController(scr_gameController _currGameController)
    {
        currGameController = _currGameController;

        scr_gameController.gameInitialPrimeSignal += initialGamePrime;
    }

    void initialGamePrime()
    {
        Debug.Log("music controller game prime started");
        StartCoroutine(initialGamePrimeCoroutine());
    }

    IEnumerator initialGamePrimeCoroutine()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        initialGamePrimedSignal?.Invoke(true);
        yield return false;
    }
}