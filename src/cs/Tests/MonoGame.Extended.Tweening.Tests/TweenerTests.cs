using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Tweening.Tests;

public class TweenerTests
{
    [Fact]
    public void TweenerTweenToSuccessTest()
    {
        var tweener = new Tweener();
        var obj = new ColorHandler();
        tweener.TweenTo(obj, x => x.Color, Color.Red, 2f);
    }
}
