using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto;


public class ScoreInputModel
{
    [Range(0, 100)]
    public int Score { get; set; }

    [Range(1900, 2099)]
    public int Year { get; set; }

    public int RankingCriteriaId { get; set; }
}