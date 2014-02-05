using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher
{
    public interface TextWriter
    {

        /**
         * Write some text.
         *
         * @param text
         */
        void WriteText(string text);

        /**
         * Write some error.
         * @param text
         */
        void WriteError(string text);

    }
}
