using Microsoft.Xna.Framework;

namespace Elizabeth
{
    public interface IAction
    {
        bool Execute(GameTime gameTime);
    };

}
