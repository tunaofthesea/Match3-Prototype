using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MatchResult
{
    List<Vector2> matches;
    bool hasEnoughMatches;
}
public interface IMatchChecker
{
    MatchResult FindMatches(GameObject go, Vector2 point);
    int getMinMatchNumber();
}
public class ThreeHorizontalMatchChecker : IMatchChecker
{
    public static final int minMatchNumber = 3;
    int getMinMatchNumber()
    {
        return this.minMatchNumber;
    }
    MatchResult FindMatches(Vector2 point)
    {
        List<GameObject> matches = new List<GameObject>(point);
        // Check vertical 
        // go left
        int col, row = point.x, point.y;
        int l, r = col, col;
        // move left as long as the left tile is the same as that of original position 
        while (l > 0 && DropMatrice[row, l - 1] != null && DropMatrice[row, col].tag == DropMatrice[row, l - 1].tag)
        {
            l--;
            matches.Add(DropMatrice[row, l]);
        }
        // move left as long as the right tile is the same as that of original position 
        while (r < columns - 1 && DropMatrice[row, r + 1] != null && DropMatrice[row, col].tag == DropMatrice[row, r + 1].tag)
        {
            r++;
            matches.Add(DropMatrice[row, r]);
        }
        bool hasEnoughMatches = matches.length() >= this.minMatchNumber;
        return MatchResult(
            matches: matches,
            hasEnoughMatches: hasEnoughMatches
        )
    }

    public void ChangeDirection(Vector2 vector2) { }
}

partial public class MatchingGame : MonoBehavior
{
    private IMatchChecker matchChecker;
    private IDestroyer destroyer;
    private ISpawner spawner;
    private IMoveAnimator moveAnimator; // moveAnimator.BlowSwipeTiles(), moveAnimator.RejectSwipe(), moveAnimator.DropEffect(), moveAnimator.Win() ;
    private IDimensionalSpace dimensionalSpace;

    public void PerformSwipeMechanics(Vector2 point)
    {
        MatchResult matchResult = this.matchChecker.FindMatches(this.matrix, point);
        if (matchResult.hasEnoughMatches)
        {
            // animate
            // destroy
            this.destroyer.Destroy(matchResult.matches);
            this.spawner.Spawn();
        }
    }
}

public class MatchingGameBuilder
{
    partial public class MatchingGame : MonoBehavior
    {
    }
    private MatchingGame matchingGame;

    public MatchingGameBuilder()
    {
        this.matchingGame = new MatchingGame();
        this.matchingGame.matchChecker = new ThreeHorizontalMatchChecker();
        // ...
    }

    public MatchingGameBuilder WithMatchChecker(IMatchChecker matchChecker)
    {
        this.matchingGame.matchChecker = matchChecker;
        return this;
    }
    public MatchingGameBuilder WithDestroyer(IDestroyer destroyer)
    {
        this.matchingGame.destroyer = destroyer;
        return this;
    }

    public MatchingGameBuilder WithDimensionalSpace(IDimensionalSpace dimensionalSpace)
    {
        this.matchingGame.dimensionalSpace = dimensionalSpace;
        return this;
    }

    public MatchingGame Create()
    {
        return this.matchingGame;
    }
}

MatchingGameBuilder builder = new MatchingGameBuilder();
Three4NeighborMatcher match = Three4NeighborMatcher();
Destroyer destroyer = new DoesNotDestroy();
MatchingGame matchingGame = builder
                    .WithMatchChecker(Three4NeighborMatcher)
                    .WithDestroyer(destroyer)
                    .Create();
