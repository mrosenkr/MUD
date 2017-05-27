namespace MUD
{
    class Program
    {
        static void Main(string[] args)
        {
            // get port from args and pass to StartListening

            Communication comm = new Communication();
            comm.StartServer();
            GameLoop(comm);
            comm.EndServer();
        }

        private static void GameLoop(Communication comm)
        {
            while (!comm.Shutdown)
            {
                comm.ProcessClientInput();

                comm.ProcessClientOutput();

                comm.ProcessDisconnects();
            }
        }

        /*
         * game loop:
         * loop through input from users and do whatever
         * loop through list of users: disconnect anyone not flagged as connected
         * loop through list of users: 
         *      tick their daze/wait if > 0, break
         *      grab input from user and put it in their command buffer, clear input from user
         *      set idle timer to 0
         * game motion (randomize the tick for fun)
         *      vote update
         *      area update
         *      song update
         *      mobile update
         *      combat update
         *      weather update
         *      object update
         *      char update
         *      room update
         *      aggro update
         * loop through list of users:
         *      send pending output that was generated from the updates and buffered on each char
         */
    }
}
