using UnityEngine;

public interface ITriggerEnterHandler {
    void HandleTriggerEnter(Collider other);
}

public interface ITriggerStayHandler {
    void HandleTriggerStay(Collider other);
}

public interface ITriggerExitHandler {
    void HandleTriggerExit(Collider other);
}