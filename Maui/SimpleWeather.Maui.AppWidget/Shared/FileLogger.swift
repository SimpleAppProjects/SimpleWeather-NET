//
//  FileLogger.swift
//  SimpleWeather
//
//  Created by Dave Antoine on 10/14/23.
//

import Foundation

struct FileGroupLogger: TextOutputStream {
    
    func getDateTimestamp() -> String {
        let formatter = DateFormatter()
        formatter.dateFormat = "yyyy-MM-dd"
        return formatter.string(from: Date())
    }

    func getLogTimestamp() -> String {
        let formatter = DateFormatter()
        formatter.dateFormat = "yyyy-MM-dd'T'HH:mm:ss.SSS"
        return formatter.string(from: Date())
    }

    func write(_ string: String) {
        let fm = FileManager.default
        if let containerUrl = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: getGroupIdentifier()) {
            let logFolderPath = containerUrl.appendingPathComponent("logs")
            
            if !fm.fileExists(atPath: logFolderPath.path()) {
                try? fm.createDirectory(at: logFolderPath, withIntermediateDirectories: true)
            }
            
            let logPath = logFolderPath.appendingPathComponent("Logger.\(getDateTimestamp()).log")
            let message = "\(getLogTimestamp())|\(string)\n"
            
            if let handle = try? FileHandle(forWritingTo: logPath) {
                handle.seekToEndOfFile()
                handle.write(message.data(using: .utf8)!)
                handle.closeFile()
            } else {
                try? message.data(using: .utf8)?.write(to: logPath)
            }
        }
    }
}

var fileGroupLogger = FileGroupLogger()
