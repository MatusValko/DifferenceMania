using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistenceManager
{


    void LoadData(GameData gameData);

    void SaveData(ref GameData gameData);




}
