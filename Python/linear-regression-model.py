#importing modules
import os
import matplotlib.pyplot as plt
import pandas as pd

#importing csv file using pandas
user_path = os.environ.get('USERPROFILE')
print(user_path)
csv_file = user_path + "\Desktop\cleanedData.csv"
print(csv_file)

# function to calculate the mean
def calculate_Mean (input):
    sum = 0
    # loops through each value in input and calculates sum of all values
    for i in range (len(input)):
        sum += input[i]
        i += 1
    # divides sum by number of values to obtain mean
    mean_value = sum/len(input)
    return mean_value
    

#subroutine to generate linear regression model for each climate variable
def linear_regression(climate_parameter):
    raw_variableData = pd.read_csv(csv_file, usecols=['STATION', 'DATE', 'LATITUDE', 'LONGITUDE', climate_parameter])

    #removes records where average temperature value is null
    if climate_parameter == 'TAVG':
        # drops record if there's no value for average temperature
        raw_variableData.dropna(subset= climate_parameter, inplace=True)
    else:
        raw_variableData.replace('', 0, inplace=True)
    print(raw_variableData)

    #holds results of linear regression
    regression_results = {}
    #states minimum level of accuracy for model 
    r_squared_threshold = 0.5

    for station, group in (raw_variableData.groupby('STATION')):
        #declare x and y values of graph
        x = group['DATE'].values
        y = group[climate_parameter].values

        #skips station if number of data points is less than 2
        if len(x) < 2:
            continue
        
        #obtain means from subroutine
        mean_x = calculate_Mean(x)
        mean_y = calculate_Mean(y)
        
        #use linear formulae to calculate slope and intercept
        numerator_value = 0
        denominator_value = 0

        for i in range (len(x)):
            numerator_value += (x[i] - mean_x)*(y[i]-mean_y)
            denominator_value += (x[i]-mean_x)** 2
        slope = numerator_value/denominator_value
        intercept = mean_y - (slope * mean_x)
        #print (f'slope = {slope}, intercept = {intercept}')

        #calculate r-squared values to eliminate insufficient models
        rss = 0
        tss = 0
        
        for i in range (len(x)):
            y_pred = slope*x[i] + intercept
            rss += (y[i] - y_pred)**2
            tss += (y[i] - mean_y)**2
        r_squared = 1 - (rss/tss)

        if r_squared > r_squared_threshold:
            regression_results[station] = {
                'slope': slope,
                'intercept': intercept,
                'latitude': group['LATITUDE'].values[0],  # Use the first value since it's the same for the group
                'longitude': group['LONGITUDE'].values[0],  # Use the first value since it's the same for the group
                'r_squared': r_squared
            }


    #plotting regression lines with latitude and longitude for each station
    for station, results in regression_results.items():
        plt.figure()
        raw_variableData['DATE'] = raw_variableData['DATE'].astype(float)
        results['slope'] = results['slope'].astype(float)

        plt.scatter(raw_variableData[raw_variableData['STATION'] == station]['DATE'], raw_variableData[raw_variableData['STATION'] == station][climate_parameter], label='Data')

        plt.plot(raw_variableData[raw_variableData['STATION'] == station]['DATE'], results['slope'] * raw_variableData[raw_variableData['STATION'] == station]['DATE'] + results['intercept'], color='red', label='Regression Line')
        plt.title(f'Linear Regression Model for Station {station}')
        
        plt.xlabel('DATE')
        plt.ylabel(climate_parameter)
        plt.text(0.1, 0.9, f'Latitude: {results["latitude"]:.2f}\nLongitude: {results["longitude"]:.2f}\nR²: {results["r_squared"]:.2f}', transform=plt.gca().transAxes, fontsize=10, verticalalignment='top')
        plt.legend()
        plt.show()

    return regression_results


def export_results (results, filename):
    df = pd.DataFrame(results).transpose()  
    df.to_csv(filename, index_label='Station')
    print(f'Regression results saved to {filename}')

prcp_results = linear_regression('PRCP')
tavg_results = linear_regression('TAVG')
snow_results = linear_regression('SNOW')

export_results(prcp_results, 'prcp_regression_results.csv')
export_results(tavg_results, 'tavg_regression_results.csv')
export_results(snow_results, 'snow_regression_results.csv')
















