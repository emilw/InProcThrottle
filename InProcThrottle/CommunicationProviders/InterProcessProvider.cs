using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Timers;

namespace InProcThrottle.CommunicationProviders
{
    public class InterProcessProvider : IClientCommunicationProvider, IManagerCommunicationProvider
    {
        IDictionary<string, MemoryMappedFile> _keyAndFileMap;
        IDictionary<string, bool> _statusCache;
        Timer _timer;



        public InterProcessProvider()
        {
            _statusCache = new Dictionary<string, bool>();
            _keyAndFileMap = new Dictionary<string, MemoryMappedFile>();
            _timer = new Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var key in _keyAndFileMap.Keys)
            {
                var latestStatus = readStatus(_keyAndFileMap[key]);
                if (_statusCache.ContainsKey(key) &&
                    (latestStatus != _statusCache[key]))
                {
                    _statusCache[key] = latestStatus;
                    StatusChanged(this, new StatusChangedEventArgs(key));
                }
            }
        }

        private MemoryMappedFile getExistingMappedFile(string tagKey)
        {
            return getMappedFile(tagKey, false);
        }

        private MemoryMappedFile getMappedFile(string tagKey, bool autoCreateIfMissing = true)
        {
            MemoryMappedFile memoryMappedFile = null;
            if (!_keyAndFileMap.ContainsKey(tagKey))
            {
                if (autoCreateIfMissing)
                    memoryMappedFile = MemoryMappedFile.CreateOrOpen(tagKey, 1024 * 1024);
                else
                    memoryMappedFile = MemoryMappedFile.OpenExisting(tagKey);

                _keyAndFileMap.Add(tagKey, memoryMappedFile);
            }
            else
            {
                memoryMappedFile = _keyAndFileMap[tagKey];
            }
            return memoryMappedFile;
        }

        private void writeStatus(MemoryMappedFile memoryMapped, bool value)
        {
            using (MemoryMappedViewStream stream = memoryMapped.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(value);
                writer.Close();
                stream.Close();
            }
        }

        private bool readStatus(MemoryMappedFile memoryMapped)
        {
            using (MemoryMappedViewStream stream = memoryMapped.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                var result = reader.ReadBoolean();
                reader.Close();
                stream.Close();
                return result;
            }
        }

        public bool DoesScopeKeyExists(string tagKey)
        {
            if (!_keyAndFileMap.ContainsKey(tagKey))
            {
                try
                {
                    var memoryMappedFile = getExistingMappedFile(tagKey);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsOkToRun(string tagKey)
        {
            if (_statusCache.ContainsKey(tagKey))
                return _statusCache[tagKey];

            var memoryMappedFile = getMappedFile(tagKey);
            var status = readStatus(memoryMappedFile);
            _statusCache.Add(tagKey,status);
            return status;
        }

        public void UpdateStatus(string tagKey, bool newStatus)
        {
            var memoryMappedFile = getMappedFile(tagKey);
            writeStatus(memoryMappedFile, newStatus);
        }

        public void Clear()
        {
            foreach (var key in _keyAndFileMap.Keys)
            {
                _keyAndFileMap[key].Dispose();
            }

            _keyAndFileMap.Clear();
        }


        public event StatusChangedEventHandler StatusChanged;
    }
}
