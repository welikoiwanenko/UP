using System;
using UPravda.Models;
namespace UPravda.DataSources
{
	interface IDataSource
	{
		News NewsData { get; set; }
	}
}
