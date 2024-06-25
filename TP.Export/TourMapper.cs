using AutoMapper;
using TP.Domain;

namespace TP.Export;

public interface TourMapper
{
    List<TourExportModel> MapTours(List<Tour> tours, bool withTourLogs);

    List<TourLogExportModel> MapTourLogs(List<TourExportModel> tourExportModels);
}

public class DefaultTourMapper(IMapper mapper) : TourMapper
{
    public List<TourExportModel> MapTours(List<Tour> tours, bool withTourLogs) => tours
                                                                                 .Select((tour, idx) => mapper.Map<TourExportModel>(tour,
                                                                                      context => { context.AfterMap((_, exportModel) => exportModel.TourNumber = idx); }))
                                                                                 .Select(tourExportModel =>
                                                                                  {
                                                                                      if (withTourLogs) return tourExportModel;
                                                                                      tourExportModel.Popularity = null;
                                                                                      tourExportModel.ChildFriendliness = null;
                                                                                      return tourExportModel;
                                                                                  })
                                                                                 .ToList();

    public List<TourLogExportModel> MapTourLogs(List<TourExportModel> tourExportModels) => tourExportModels.SelectMany(tourExportModel => mapper.Map<List<TourLogExportModel>>(tourExportModel.TourLogs)
                                                                                                                                                .Select(tourLogExportModel =>
                                                                                                                                                 {
                                                                                                                                                     tourLogExportModel.TourNumber = tourExportModel.TourNumber;
                                                                                                                                                     return tourLogExportModel;
                                                                                                                                                 }))
                                                                                                           .ToList();
}