using CsvImporter.Domain.Interfaces;
using CsvImporter.Infraestructure.Services;
using System;
using System.Text.RegularExpressions;

namespace CsvImporter.Infraestructure
{
    /// <summary>
    /// Factory para implementaciones de IGetFileFactory. 
    /// </summary>
    public class GetFileFactory : IGetFileFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GetFileFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Devuelve una implementacion de IGetFileFactory dependiendo de si el parametro
        /// sourceFile es una URL.
        /// </summary>
        /// <param name="sourceFile">Origen del archivo.</param>
        /// <returns></returns>
        public IGetFile GetFileProvider(string sourceFile)
        {
            if (Uri.IsWellFormedUriString(sourceFile, UriKind.Absolute))
            {
                return (IGetFile)_serviceProvider.GetService(typeof(GetFileFromUrl));
            }
            else
            {
                return (IGetFile)_serviceProvider.GetService(typeof(GetFile));
            }
        }

        
    }
}
