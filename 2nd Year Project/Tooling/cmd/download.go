package main

import (
	"bufio"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"os"
	"strconv"
	"strings"
)

type School struct {
	// Only need to store a schools ID (URN) and the website address, other info isn't needed in this scope
	URN int
	SchoolWebsite string
}

func main() {
	// opens strip.json from the workind dir
	inputFile := "./strip.json"

	schools, err := GetSchools(inputFile)
	if err != nil {
		fmt.Println(err)
		return
	}

	for _, school := range schools {
		// Download the bytes from a website
		bytes, err := GetBytes(&school)
		if err != nil {
			fmt.Printf("Failed to get bytes for %d: %s\n", school.URN, err.Error())
			continue
		}
		// Convert int URN to a string
		path := strconv.Itoa(school.URN) + ".html"
		// Save websites bytes to file path
		err = SaveBytes(path, bytes)
		if err != nil {
			fmt.Printf("Failed to save bytes for %d: %s\n", school.URN, err.Error())
			continue
		}
		fmt.Printf("Downloaded %d (%s)\n", school.URN, school.SchoolWebsite)
	}
}

// Loads schools from a file
func GetSchools(path string) ([]School, error) {
	file, err := os.Open(path)
	if err != nil {
		return nil, err
	}

	scanner := bufio.NewScanner(file)
	if err := scanner.Err(); err != nil {
		return nil, err
	}

	fileContents := ""
	for scanner.Scan() {
		fileContents += scanner.Text() + "\n"
	}
	fileBytes := []byte(fileContents)

	var schools []School
	// Unmarshal takes the bytes of json and converts it into school structs
	err = json.Unmarshal(fileBytes, &schools)
	if err != nil {
		return nil, err
	}

	for i := range schools {
		// Not all schools websites are prefixed http://, which will cause a protocol error as the downloader is unsure what to use
		if !strings.HasPrefix(schools[i].SchoolWebsite, "http") {
			schools[i].SchoolWebsite = "http://" + schools[i].SchoolWebsite
		}
		fmt.Printf("")
	}

	return schools, nil
}

func GetBytes(school *School) ([]byte, error) {
	response, err := http.Get(school.SchoolWebsite)
	if err != nil {
		return nil, err
	}

	// Defer closing the response's body until after the ReadAll has read it
	defer response.Body.Close()
	// Read the response to a string
	return ioutil.ReadAll(response.Body)
}

func SaveBytes(path string, bytes []byte) error {
	file, err := os.Create(path)
	if err != nil {
		return err
	}
	// Defers closing the file in case an error occurs during writing
	defer file.Close()

	_, err = file.Write(bytes)
	if err != nil {
		return err
	}

	return nil
}