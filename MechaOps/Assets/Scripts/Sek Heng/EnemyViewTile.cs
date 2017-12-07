using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will be used to render this unit invisible! inherit from PlayerViewScript to be lazy
/// </summary>
public class EnemyViewTile : ViewTileScript
{
    public override void RenderSurroundingTiles()
    {
        throw new System.NotImplementedException();
    }
}
